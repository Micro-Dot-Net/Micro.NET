using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Micro.Net.Dispatch;
using Micro.Net.Handling;
using Micro.Net.Transport.Generic;

namespace Micro.Net.Transport.Tcp
{
    public class TcpDispatcher : GenericDispatcherBase
    {
        private readonly TcpDispatcherConfiguration _config;
        private readonly IMicroSystemConfiguration _systemConfig;

        private readonly IDictionary<(string, int), TcpDispatchModule> _modules = new ConcurrentDictionary<(string, int), TcpDispatchModule>();

        public override ISet<DispatcherFeature> Features => new HashSet<DispatcherFeature> { };
        public override IEnumerable<(Type, Type)> Available { get; }

        public TcpDispatcher(TcpDispatcherConfiguration config, IMicroSystemConfiguration systemConfig)
        {
            _config = config;
            _systemConfig = systemConfig;
        }

        public override async Task Handle<TRequest, TResponse>(IDispatchContext<TRequest, TResponse> messageContext)
        {
            throw new NotImplementedException();
        }

        public override Task Start(CancellationToken cancellationToken)
        {

        }

        public override Task Stop(CancellationToken cancellationToken)
        {

        }
    }

    public class TcpDispatcherConfiguration
    {
        public IDictionary<(string Host, int Port), TcpDispatchRoute> Routes { get; set; }
    }

    public class TcpDispatchRoute
    {
        public Dictionary<Type, Type> TypeMaps { get; set; }
        public bool IsSecure { get; set; }
        public string[] AcceptThumbprints { get; set; }
        public X509Certificate2 Certificate { get; set; }
    }
}
