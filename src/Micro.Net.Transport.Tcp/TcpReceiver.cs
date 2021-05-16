using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Micro.Net.Abstractions.Transport;
using Micro.Net.Core.Abstractions.Pipeline;
using Micro.Net.Core.Extensions;
using Micro.Net.Exceptions;
using Micro.Net.Receive;
using Micro.Net.Transport.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;

namespace Micro.Net.Transport.Tcp
{
    public class TcpReceiver : GenericReceiverBase
    {
        private readonly TcpReceiverConfiguration _config;
        private readonly ICollection<TcpReceiverModule> _receiverModules = new List<TcpReceiverModule>();
        private readonly JsonSerializer _serializer;

        public TcpReceiver(IPipeChannel pipeChannel, TcpReceiverConfiguration config) : base(pipeChannel)
        {
            _config = config;

            _serializer = new JsonSerializer() {TypeNameHandling = TypeNameHandling.Objects};

            foreach (KeyValuePair<int, TcpReceiverRoute> tcpReceiverRoute in config.Routes)
            {
                TcpReceiverModule module;

                _receiverModules.Add(module = new TcpReceiverModule(new IPEndPoint(IPAddress.Any, tcpReceiverRoute.Key), _serializer, tcpReceiverRoute.Value.SecureOptions));

                module.OnReceive += async (args) => await ModuleOnOnReceive(args, tcpReceiverRoute.Value);
            }
        }

        private async Task ModuleOnOnReceive(TcpModule.MessageReceivedArgs args, TcpReceiverRoute route)
        {
            object envelope = args.Message.ToObject(typeof(object), _serializer);

            Type envelopeType = typeof(Envelope<>);

            if (envelope.GetType().GetGenericTypeDefinition() != envelopeType)
            {
                //return error -- must be an envelope!
                return;
            }

            Type messageType = envelope.GetType().GenericTypeArguments.First();
            Type responseType = route.TypeMap[messageType];

            if (responseType == null)
            {
                //return error -- unmapped type!
                return;
            }

            MethodInfo procMethod = _methodCache[(messageType, responseType)];

            if (procMethod == null)
            {
                procMethod = GetType().GetMethod(nameof(Process)).MakeGenericMethod(messageType, responseType);

                _methodCache[(messageType, responseType)] = procMethod;
            }

            Envelope response = (Envelope)procMethod.Invoke(this, new object[] {envelope});

            JObject responseObj = JObject.FromObject(response, _serializer);

            args.Reply(responseObj);
        }

        private Dictionary<(Type, Type), MethodInfo> _methodCache = new Dictionary<(Type, Type), MethodInfo>();



        private Envelope<TResponse> Process<TRequest, TResponse>(Envelope<TRequest> request)
        {
            IReceiveContext<TRequest, TResponse> context = ReceiveContext.Create<TRequest, TResponse>(request);
            
            Dispatch(context).RunSynchronously();

            return new Envelope<TResponse>()
            {
                Headers = context.Response.Headers,
                Message = context.Response.Payload
            };
        }

        public override async Task Start(CancellationToken cancellationToken)
        {
            foreach (TcpReceiverModule receiverModule in _receiverModules)
            {
                await receiverModule.Start(cancellationToken);
            }
        }

        public override async Task Stop(CancellationToken cancellationToken)
        {
            foreach (TcpReceiverModule receiverModule in _receiverModules)
            {
                await receiverModule.Stop(cancellationToken);
            }
        }
    }

    public class TcpReceiverConfiguration
    {
        public Dictionary<int, TcpReceiverRoute> Routes { get; set; }
    }

    public class TcpReceiverRoute
    {
        public Dictionary<Type, Type> TypeMap { get; set; }
        public SecureTcpOptions SecureOptions { get; set; }
    }
}
