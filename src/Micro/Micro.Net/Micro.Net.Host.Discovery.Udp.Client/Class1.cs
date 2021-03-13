using System;
using System.Threading.Tasks;
using Micro.Net.Abstractions.Components;
using Micro.Net.Abstractions.Discovery;

namespace Micro.Net.Host.Discovery.Udp.Client
{
    public class UdpDiscoveryClient : DiscoveryService
    {
        public async Task Initialize()
        {
            throw new NotImplementedException();
        }

        public async Task Start()
        {
            throw new NotImplementedException();
        }

        public async Task Stop()
        {
            throw new NotImplementedException();
        }

        public event ServiceEventDelegate ServiceDiscovered;
        public event ServiceEventDelegate ServiceLost;
        public void Announce()
        {
            throw new NotImplementedException();
        }

        public void Goodbye()
        {
            throw new NotImplementedException();
        }
    }

    public class ClientConfiguration
    {

    }
}
