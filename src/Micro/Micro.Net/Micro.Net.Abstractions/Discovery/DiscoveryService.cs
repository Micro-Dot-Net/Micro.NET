using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Micro.Net.Abstractions.Components;

namespace Micro.Net.Abstractions.Discovery
{
    public interface DiscoveryService : IComponent<ComponentKind.Discovery>
    {
        event ServiceEventDelegate ServiceDiscovered;
        event ServiceEventDelegate ServiceLost;

        void Enlist(Uri serviceAddress, string contractName, string assemblyName);

        void Announce();
        void Goodbye();
    }
}
