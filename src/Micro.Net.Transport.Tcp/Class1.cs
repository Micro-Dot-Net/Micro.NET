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

namespace Micro.Net.Transport.Tcp
{
    public class TcpReceiver : GenericReceiverBase
    {
        private readonly TcpReceiverConfiguration _config;
        private List<TcpListener> _listeners = new List<TcpListener>();
        
        private JsonSerializer _serializer = new JsonSerializer(){TypeNameHandling = TypeNameHandling.All};

        public TcpReceiver(TcpReceiverConfiguration config, IPipeChannel pipeChannel) : base(pipeChannel)
        {
            _config = config;
        }

        public override async Task Start(CancellationToken cancellationToken)
        {
            foreach (KeyValuePair<int, TcpReceiveRoute> configGroup in _config.Routes)
            {
                Thread listenThread = new Thread(async () => await listen(configGroup.Key, configGroup.Value, cancellationToken)) {IsBackground = true};

                listenThread.Start();
            }
        }

        private async Task listen(int port, TcpReceiveRoute routeConfiguration, CancellationToken cancellationToken)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);

            _listeners.Add(listener);

            listener.Start();

            CancellationTokenSource source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            while (cancellationToken.WaitHandle.WaitOne(0))
            {
                TcpClient client = await listener.AcceptTcpClientAsync();

                processConnection(client, routeConfiguration, source.Token);
            }
        }

        private async Task processConnection(TcpClient client, TcpReceiveRoute routeConfig, CancellationToken cancellationToken)
        {
            Stream clientStream = client.GetStream();

            if (routeConfig.IsSecure)
            {
                SslStream secureStream = new SslStream(clientStream, false, 
                    (sender, certificate, chain, errors) => remoteValidationCallback(sender, routeConfig, certificate, chain, errors),
                    (sender, host, certificates, certificate, issuers) => localSelectionCallback(sender, host, routeConfig, certificates, certificate, issuers));

                clientStream = secureStream;
            }
            
            SemaphoreSlim sema = new SemaphoreSlim(0,1);

            using (client)
            {
                while (cancellationToken.WaitHandle.WaitOne(0))
                {
                    object request;

                    using (BsonDataReader reader = new BsonDataReader(clientStream))
                    {
                        request = _serializer.Deserialize(reader);
                    }

#pragma warning disable 4014
                    processMessage(request, routeConfig, cancellationToken).ContinueWith(async (Task<object> task) =>
#pragma warning restore 4014
                    {
                        object response = task.GetAwaiter().GetResult();

                        await sema.WaitAsync(cancellationToken);

                        try
                        {
                            using (BsonDataWriter writer = new BsonDataWriter(clientStream))
                            {
                                _serializer.Serialize(writer, response);
                            }
                        }
                        finally
                        {
                            sema.Release();
                        }
                    }, cancellationToken);
                }
            }
        }

        private bool remoteValidationCallback(object sender, TcpReceiveRoute routeConfig, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslpolicyerrors)
        {
            if (certificate != null && chain != null)
            {
                X509Certificate2 cert = new X509Certificate2(certificate);

                return routeConfig.AcceptThumbprints.Contains(cert.Thumbprint) && chain.Build(cert);
            }

            return false;
        }

        private X509Certificate localSelectionCallback(object sender, string targethost, TcpReceiveRoute routeConfig, X509CertificateCollection localcertificates, X509Certificate? remotecertificate, string[] acceptableissuers)
        {
            return routeConfig.Certificate;
        }

        public IDictionary<(Type, Type), MethodInfo> pipeCache = new ConcurrentDictionary<(Type, Type), MethodInfo>();

        private async Task<object> processMessage(object message, TcpReceiveRoute routeConfig, CancellationToken cancellationToken)
        {
            if (message.GetType().GetGenericTypeDefinition() != typeof(Envelope<>))
            {
                return new Envelope<Exception>()
                {
                    Headers = new Dictionary<string, string[]>
                    {

                    }, 
                    Message = new MicroReceiverException("Received message is not an Envelope!",-99)
                };
            }

            Type reqType = message.GetType().GetGenericArguments()[0];
            Type respType = routeConfig.TypeMaps[reqType];

            MethodInfo method;

            if ((method = pipeCache[(reqType, respType)]) == null)
            {
                pipeCache[(reqType, respType)] = method = this.GetType().GetMethod(nameof(dispatch)).MakeGenericMethod(new Type[] {reqType, respType});
            }

            return await (Task<object>)method.Invoke(this, new object[] { message });
        }

        private async Task<object> dispatch<TRequest,TResponse>(Envelope<TRequest> envelope)
        {
            IReceiveContext<TRequest, TResponse> context = ReceiveContext.Create<TRequest, TResponse>(envelope);

            await base.Dispatch(context);

            Envelope<TResponse> responseEnv = new Envelope<TResponse>()
            {
                Headers = context.Response.Headers,
                Message = context.Response.Payload
            };

            return responseEnv;
        }

        public override async Task Stop(CancellationToken cancellationToken)
        {
            foreach (TcpListener listener in _listeners)
            {
                listener.Stop();
            }
        }
    }

    public class TcpReceiverConfiguration
    {
        public IDictionary<int, TcpReceiveRoute> Routes { get; set; }
        public Type DefaultWireFormatter { get; set; }
    }

    public class TcpReceiveRoute
    {
        public Dictionary<Type,Type> TypeMaps { get; set; }
        public bool IsSecure { get; set; }
        public string[] AcceptThumbprints { get; set; }
        public X509Certificate2 Certificate { get; set; }
    }

    public interface IWireFormatter
    {
        
    }
}
