using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using Micro.Net.Abstractions;
using Micro.Net.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Micro.Net.Receive.Http
{
    public class HttpReceiver : IStartable, IStoppable
    {
        private readonly IMediator _mediator;
        private readonly HttpListener _listener;
        private readonly HttpReceiverConfiguration _configuration;

        public HttpReceiver(IMediator mediator, HttpListener listener, HttpReceiverConfiguration configuration)
        {
            _mediator = mediator;
            _listener = listener;
            _configuration = configuration;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            if (!HttpListener.IsSupported)
            {
                throw InitializationException.FeatureNotSupported("HttpListener");
            }

            foreach (string baseUri in _configuration.BaseUris)
            {
                _listener.Prefixes.Add(baseUri);
            }

            _listener.Start();

            Task.Run(Run);
        }

        private async Task Run()
        {
            while (true)
            {
                HttpListenerContext context = await _listener.GetContextAsync();

                Task.Run(async () => Handle(context));
            }
        }

        private async Task Handle(HttpListenerContext listenerContext)
        {
            try
            {
                (Type reqType, Type respType) = _configuration.PathMaps[listenerContext.Request.Url.AbsolutePath];

                dynamic context = _fabricate(reqType, respType, listenerContext.Request.Headers);
                //ReceiveContext<dynamic, dynamic> context = null;

                foreach (string key in listenerContext.Request.Headers.AllKeys)
                {
                    context.Request.Headers[key] = listenerContext.Request.Headers[key];
                }

                if (!listenerContext.Request.HasEntityBody)
                {
                    var dict = HttpUtility.ParseQueryString(listenerContext.Request.QueryString?.ToString() ??
                                                            string.Empty);
                    JObject json = JObject.FromObject(dict.Cast<string>().ToDictionary(k => k, v => dict[v]));

                    context.Request.Payload = (dynamic)json.ToObject(reqType);
                }
                else
                {
                    JObject json;

                    using (StreamReader reader = new StreamReader(listenerContext.Request.InputStream,
                        listenerContext.Request.ContentEncoding))
                    {
                        json = JObject.Parse(reader.ReadToEnd());
                    }

                    context.Request.Payload = (dynamic)json.ToObject(reqType);
                }

                context.Destination = listenerContext.Request.Url;

                if (listenerContext.Request.Headers["Source"] != null)
                {
                    context.Source = new Uri(listenerContext.Request.Headers["Source"]);
                }
                else if (listenerContext.Request.RemoteEndPoint != null)
                {
                    string host;

                    try
                    {
                        IPHostEntry dns = await Dns.GetHostEntryAsync(listenerContext.Request.RemoteEndPoint.Address);

                        host = $"{dns.HostName}:{listenerContext.Request.RemoteEndPoint.Port}";
                    }
                    catch (SocketException ex) when (ex.ErrorCode == 11001)
                    {
                        host = listenerContext.Request.RemoteEndPoint.ToString();
                    }

                    context.Source = new Uri($"http://{host}");
                }
                else
                {
                    context.Source = new Uri("null://");
                }

                try
                {
                    await _mediator.Send(context);

                    foreach (KeyValuePair<string, string> header in context.Response.Headers)
                    {
                        listenerContext.Response.Headers.Add(header.Key, header.Value);
                    }

                    if (respType != typeof(ValueTuple))
                    {
                        listenerContext.Response.StatusCode = 200;
                        listenerContext.Response.StatusDescription = "OK";

                        JObject payload = JObject.FromObject(context.Response.Payload);

                        listenerContext.Response.Close(
                            listenerContext.Request.ContentEncoding.GetBytes(
                                payload.ToString(Formatting.Indented)),
                            true);
                    }
                    else
                    {
                        listenerContext.Response.StatusCode = 204;
                        listenerContext.Response.StatusDescription = "No Content";

                        listenerContext.Response.Close();
                    }
                }
                catch (MicroReceiverException ex) when (ex.HResult == 404)
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
            }
            catch (Exception ex)
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
            }
        }



        public object _fabricate(Type requestType, Type responseType, NameValueCollection headers)
        {
            Type contextType = typeof(ReceiveContext<,>).MakeGenericType(requestType, responseType);
            Type reqCtxType = typeof(RequestContext<>).MakeGenericType(requestType);
            Type respCtxType = typeof(ResponseContext<>).MakeGenericType(responseType);

            dynamic context = Activator.CreateInstance(contextType);
            context.Request = (dynamic)Activator.CreateInstance(reqCtxType);
            context.Request.Headers = new Dictionary<string, string>();
            context.Response = (dynamic)Activator.CreateInstance(respCtxType);
            context.Response.Headers = new Dictionary<string, string>();

            return context;
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            _listener.Stop();
        }
    }
}