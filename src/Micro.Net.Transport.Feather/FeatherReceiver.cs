using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Micro.Net.Abstractions;
using Micro.Net.Core.Abstractions.Pipeline;
using Micro.Net.Exceptions;
using Micro.Net.Receive;
using Micro.Net.Transport.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Micro.Net.Transport.Feather
{
    public class FeatherReceiver : GenericReceiverBase
    {
        private readonly FeatherReceiverConfiguration _config;
        private readonly IContextFactory _contextFactory;
        private readonly ILogger<FeatherReceiver> _logger;
        private readonly WebApplication _app;

        public FeatherReceiver(IPipeChannel pipeChannel, FeatherReceiverConfiguration config, IContextFactory contextFactory, ILogger<FeatherReceiver> logger) : base(pipeChannel)
        {
            _config = config;
            _contextFactory = contextFactory;            
            _logger = logger;
            _app = WebApplication.Create();
            _app.Listen(_config.BaseUris.ToArray());
            
            foreach(KeyValuePair<(string path, HttpMethod verb),(Type request,Type response)> mapping in config.PathMaps)
            {
                MethodInfo handlerMtd = GetType().GetMethod(nameof(Handle), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(mapping.Value.request, mapping.Value.response);

                _logger.LogDebug($"Setting up path ({mapping.Key.verb})'{mapping.Key.path}'...");

                switch (mapping.Key.verb)
                {
                    case HttpMethod m when m == HttpMethod.Get:
                        _app.MapGet(mapping.Key.path, async ctx => 
                        {
                            _logger.LogInformation($"Request received! ({mapping.Key.path})");
                            await (Task)handlerMtd.Invoke(this, new object[] { ctx });
                        });
                        break;
                    case HttpMethod m when m == HttpMethod.Post:
                        _app.MapPost(mapping.Key.path, async ctx =>
                        {
                            _logger.LogInformation($"Request received! ({mapping.Key.path})");
                            await (Task)handlerMtd.Invoke(this, new object[] { ctx });
                        });
                        break;
                    default:
                        throw new Micro.Net.Exceptions.MicroReceiverException("Only GET and POST methods are supported on this transport!", 422);
                }
            }

            _app.MapFallback(async ctx => 
            {
                await ctx.Response.WriteAsync("Fallback!");
            });
        }

        private async Task Handle<TRequest,TResponse>(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();

            try
            {

                IReceiveContext<TRequest, TResponse> context;

                if (!_contextFactory.TryCreate(out context))
                {
                    throw new MicroConfigurationException();
                }

                foreach (string key in httpContext.Request.Headers.Keys)
                {
                    context.Request.Headers[key] = httpContext.Request.Headers[key].ToArray();
                }

                if (httpContext.Request.ContentLength <= 0)
                {
                    var dict = HttpUtility.ParseQueryString(httpContext.Request.QueryString.ToString());
                    JObject json = JObject.FromObject(dict.Cast<string>().ToDictionary(k => k, v => dict[v]));

                    context.Request.Payload = json.ToObject<TRequest>();
                }
                else
                {
                    JObject json;

                    using (StreamReader reader = new StreamReader(httpContext.Request.Body))
                    {
                        json = JObject.Parse(await reader.ReadToEndAsync());
                    }

                    context.Request.Payload = json.ToObject<TRequest>();
                }

                context.Destination = new Uri(UriHelper.GetEncodedUrl(httpContext.Request));

                if (httpContext.Request.Headers.ContainsKey("Source"))
                {
                    context.Source = new Uri(httpContext.Request.Headers["Source"]);
                }
                else if (httpContext.Connection.RemoteIpAddress != null)
                {
                    string host;

                    try
                    {
                        IPHostEntry dns = await Dns.GetHostEntryAsync(httpContext.Connection.RemoteIpAddress);

                        host = $"{dns.HostName}:{httpContext.Connection.RemotePort}";
                    }
                    catch (Exception ex)
                    {
                        host = $"{httpContext.Connection.RemoteIpAddress}:{httpContext.Connection.RemotePort}";
                    }

                    
                    context.Source = new Uri($"http://{host}");
                }
                else
                {
                    context.Source = new Uri("null://");
                }

                try
                {
                    await base.Dispatch(context);

                    foreach (KeyValuePair<string, string[]> header in context.Response.Headers)
                    {
                        httpContext.Response.Headers.Add(header.Key, header.Value);
                    }

                    if (typeof(TResponse) != typeof(ValueTuple))
                    {
                        httpContext.Response.StatusCode = 200;

                        //JObject payload = JObject.FromObject(context.Response.Payload);

                        await httpContext.Response.WriteAsJsonAsync<TResponse>(context.Response.Payload);
                    }
                    else
                    {
                        httpContext.Response.StatusCode = 204;
                    }
                }
                catch (MicroReceiverException ex) when (ex.HResult == 404)
                {
                    httpContext.Response.StatusCode = 404;
                }
                catch (AggregateException ex)
                {
                    httpContext.Response.StatusCode = 500;

                    //JObject exObj = JObject.FromObject(ex);

#if DEBUG
                    await httpContext.Response.WriteAsJsonAsync<AggregateException>(ex);
#endif
                }
                catch (TaskCanceledException)
                {
                    httpContext.Response.StatusCode = 444;
                }
            }
            catch (Exception ex)
            {
                httpContext.Response.StatusCode = 500;

                JObject exObj = JObject.FromObject(ex);

#if DEBUG
                await httpContext.Response.WriteAsync(exObj.ToString());
#endif
            }

            await httpContext.Response.CompleteAsync();
        }

        public override async Task Start(CancellationToken cancellationToken)
        {
            await _app.StartAsync(cancellationToken);
        }

        public override async Task Stop(CancellationToken cancellationToken)
        {
            await _app.StopAsync(cancellationToken);
        }
    }
}