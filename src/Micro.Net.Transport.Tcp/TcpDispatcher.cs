using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Transport;
using Micro.Net.Dispatch;
using Micro.Net.Handling;
using Micro.Net.Transport.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Micro.Net.Transport.Tcp
{
    public class TcpDispatcher : GenericDispatcherBase
    {
        private readonly TcpDispatcherConfiguration _config;

        public override ISet<DispatcherFeature> Features { get; } = new HashSet<DispatcherFeature>() {DispatcherFeature.Replies};
        public override IEnumerable<(Type, Type)> Available { get; }

        private readonly IDictionary<(Type,Type), TcpDispatcherModule> _modules;
        private JsonSerializer _serializer;

        public TcpDispatcher(TcpDispatcherConfiguration config)
        {
            _config = config;
            Available = config.Routes.Values.Select(x => (x.RequestType, x.ResponseType));

            _serializer = new JsonSerializer() {TypeNameHandling = TypeNameHandling.Objects};

            _modules = config.Routes.Values
                .ToDictionary(x => (x.RequestType, x.ResponseType), 
                    y => new TcpDispatcherModule(y.Endpoint, _serializer, y.SecureOptions));
        }

        public override async Task Handle<TRequest, TResponse>(IDispatchContext<TRequest, TResponse> messageContext)
        {
            Type reqType = typeof(TRequest);
            Type respType = typeof(TResponse);

            TcpDispatcherModule module = _modules[(reqType, respType)];

            Envelope<TRequest> requestEnvelope = new Envelope<TRequest>()
            {
                Message = messageContext.Request.Payload,
                Headers = messageContext.Request.Headers
            };

            TaskCompletionSource<JObject> responseSource =
                new TaskCompletionSource<JObject>(TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);

            await module.Send(JObject.FromObject(requestEnvelope), jobj => responseSource.SetResult(jobj));

            JObject response = await responseSource.Task;

            Envelope env = response.ToObject<Envelope>(_serializer);

            if (env is Envelope<Exception> exEnv)
            {
                messageContext.SetFault(exEnv.Message);
                messageContext.Response.Headers = exEnv.Headers;

                return;
            }

            if (env is Envelope<TResponse> respEnv)
            {
                messageContext.Response.Payload = respEnv.Message;
                messageContext.Response.Headers = respEnv.Headers;
                messageContext.SetResolve();

                return;
            }
        }

        public override async Task Start(CancellationToken cancellationToken)
        {
            await Task.WhenAll(_modules.Values.Select(x => x.Start(cancellationToken)));
        }

        public override async Task Stop(CancellationToken cancellationToken)
        {
            await Task.WhenAll(_modules.Values.Select(x => x.Stop(cancellationToken)));
        }
    }

    public class TcpDispatcherConfiguration
    {
        public IDictionary<Type, TcpDispatchRoute> Routes { get; set; }
    }

    public class TcpDispatchRoute
    {
        public IPEndPoint Endpoint { get; set; }
        public SecureTcpOptions SecureOptions { get; set; }
        public Type RequestType { get; set; }
        public Type ResponseType { get; set; }
    }
}
