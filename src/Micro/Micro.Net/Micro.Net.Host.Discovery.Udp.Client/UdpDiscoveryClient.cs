using System;
using System.Threading;
using System.Threading.Tasks;
using Micro.Net.Abstractions.Components;
using Micro.Net.Abstractions.Discovery;

namespace Micro.Net.Host.Discovery.Udp.Client
{
    public class UdpDiscoveryClient : DiscoveryService
    {
        public event ServiceEventDelegate ServiceDiscovered;
        public event ServiceEventDelegate ServiceLost;

        public void Enlist(Uri serviceAddress, string contractName, string assemblyName)
        {
            throw new NotImplementedException();
        }

        public void Announce()
        {
            throw new NotImplementedException();
        }

        public void Goodbye()
        {
            throw new NotImplementedException();
        }

        public async Task Initialize(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
