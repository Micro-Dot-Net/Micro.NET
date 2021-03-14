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
using Micro.Net.Abstractions.Messages;
using Micro.Net.Abstractions.Messages.Receive;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Micro.Net.Host.Http
{
    public class HttpReceiverService : ReceiverService
    {
        private readonly HttpReceiverOptions _opts;
        private readonly HttpListener _listener;

        public HttpReceiverService(HttpReceiverOptions opts, HttpListener listener)
        {
            _opts = opts;
            _listener = listener;
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
                HttpListenerContext context = await _listener.GetContextAsync();

                TaskCompletionSource<TransportMessage> completion =
                    new TaskCompletionSource<TransportMessage>(TaskCreationOptions.LongRunning);

                TransportMessage message = synthesizeMessage(context.Request);

                TransportEnvelope envelope = new TransportEnvelope(completion) {Message = message};

                Task.Run(() => Wait(context, completion.Task));

                OnReceive?.Invoke(envelope);
            }
        }

        private TransportMessage synthesizeMessage(HttpListenerRequest request)
        {
            TransportMessage message = new TransportMessage(){ Headers = new Dictionary<string, string>() };

            foreach (string key in request.Headers.AllKeys)
            {
                message.Headers[key] = request.Headers[key];
            }

            if (!request.HasEntityBody)
            {
                var dict = HttpUtility.ParseQueryString(request.QueryString?.ToString() ?? string.Empty);
                JObject json = JObject.FromObject(dict.Cast<string>().ToDictionary(k => k, v => dict[v]));

                message.Body = json;
            }
            else
            {
                JObject json;

                using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    json = JObject.Parse(reader.ReadToEnd());
                }

                message.Body = json;
            }

            message.Origin = request.Url;

            return message;
        }

        private async Task Wait(HttpListenerContext context, Task<TransportMessage> replyTask)
        {
            try
            {
                TransportMessage reply = await replyTask;

                context.Response.StatusCode = reply.Body.HasValues ? 200 : 204;
                context.Response.StatusDescription = reply.Body.HasValues ? "OK" : "No Content";

                foreach(KeyValuePair<string, string> header in reply.Headers)
                {
                    context.Response.Headers.Add(header.Key, header.Value);
                }

                context.Response.Close(context.Request.ContentEncoding.GetBytes(reply.Body.ToString(Formatting.None)),true);
            }
            catch (AggregateException ex)
            {
                context.Response.StatusCode = 500;
                context.Response.StatusDescription = "Internal Server Error";

                //Serialize error?
            }
            catch (TaskCanceledException)
            {
                context.Response.StatusCode = 444;
                context.Response.StatusDescription = "No Response";

                using (StreamWriter writer = new StreamWriter(context.Response.OutputStream){AutoFlush = true})
                {
                    await writer.WriteLineAsync("Task cancelled!");
                }
            }
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            _listener.Stop();
        }

        public event Action<TransportEnvelope> OnReceive;
    }

    public class HttpReceiverOptions
    {
        public string[] Prefixes { get; set; }
    }
}