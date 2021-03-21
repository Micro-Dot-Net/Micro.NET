using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Exceptions;
using Micro.Net.Abstractions.Messages;
using Micro.Net.Abstractions.Messages.Receive;
using Micro.Net.Abstractions.Processing;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Micro.Net.Host.Http
{
    public class HttpReceiverService : ReceiverService
    {
        private readonly HttpReceiverOptions _opts;
        private readonly HttpListener _listener;
        private readonly IPipelineHead<HttpReceiveContext> _pipeHead;

        public HttpReceiverService(HttpReceiverOptions opts, HttpListener listener, IPipelineHead<HttpReceiveContext> pipeHead)
        {
            _opts = opts;
            _listener = listener;
            _pipeHead = pipeHead;
        }

        public async Task Initialize(CancellationToken cancellationToken)
        {
            if (!HttpListener.IsSupported)
            {
                throw InitializationException.FeatureNotSupported("HttpListener");
            }

            foreach (string prefix in _opts.Prefixes)
            {
                _listener.Prefixes.Add(prefix);
            }

            //Perform other configuration
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            _listener.Start();

            Task.Run(() => Run(cancellationToken), cancellationToken);
        }

        private async Task Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                HttpListenerContext contextTask = await _listener.GetContextAsync();

                Task.Run(async () =>
                {
                    HttpListenerContext listenerContext = contextTask;
                    
                    HttpReceiveContext context = new HttpReceiveContext();

                    foreach (string key in listenerContext.Request.Headers.AllKeys)
                    {
                        context.HttpRequest.Headers[key] = listenerContext.Request.Headers[key];
                    }

                    if (!listenerContext.Request.HasEntityBody)
                    {
                        var dict = HttpUtility.ParseQueryString(listenerContext.Request.QueryString?.ToString() ?? string.Empty);
                        JObject json = JObject.FromObject(dict.Cast<string>().ToDictionary(k => k, v => dict[v]));

                        context.HttpRequest.Body = json;
                    }
                    else
                    {
                        JObject json;

                        using (StreamReader reader = new StreamReader(listenerContext.Request.InputStream, listenerContext.Request.ContentEncoding))
                        {
                            json = JObject.Parse(reader.ReadToEnd());
                        }

                        context.HttpRequest.Body = json;
                    }

                    context.HttpRequest.Origin = listenerContext.Request.Url;

                    try
                    {
                        await _pipeHead.Execute(context);

                        listenerContext.Response.StatusCode = context.HttpResponse.Body.HasValues ? 200 : 204;
                        listenerContext.Response.StatusDescription =
                            context.HttpResponse.Body.HasValues ? "OK" : "No Content";

                        foreach (KeyValuePair<string, string> header in context.HttpResponse.Headers)
                        {
                            listenerContext.Response.Headers.Add(header.Key, header.Value);
                        }

                        listenerContext.Response.Close(
                            listenerContext.Request.ContentEncoding.GetBytes(
                                context.HttpResponse.Body.ToString(Formatting.None)),
                            true);
                    }
                    catch (MicroTransportException ex)
                    {
                        listenerContext.Response.StatusCode = 404;
                        listenerContext.Response.StatusDescription = "Not Found";
                        
                        listenerContext.Response.Close();
                    }
                    catch (AggregateException ex)
                    {
                        listenerContext.Response.StatusCode = 500;
                        listenerContext.Response.StatusDescription = "Internal Server Error";
                        
                        JObject exObj = JObject.FromObject(ex);

                        #if DEBUG
                        listenerContext.Response.Close(
                            listenerContext.Request.ContentEncoding.GetBytes(exObj.ToString(Formatting.Indented)), 
                            true);
                        #else
                        listenerContext.Response.Close();
                        #endif

                        //Serialize error?
                    }
                    catch (TaskCanceledException)
                    {
                        listenerContext.Response.StatusCode = 444;
                        listenerContext.Response.StatusDescription = "Task cancelled";
                        
                        listenerContext.Response.Close();
                    }
                });
            }
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            _listener.Stop();
        }
    }
}