using System;
using Micro.Net.Abstractions.Discovery;

namespace Micro.Net.Abstractions.Configuration
{
    public interface IMicroserviceConfigurable
    {
        IMicroserviceConfigurable ConfigureReceivers(Action<IReceiverConfigurable> config);
        IMicroserviceConfigurable ConfigureDispatchers(Action<IDispatcherConfigurable> config);
        IMicroserviceConfigurable ConfigureHandlers(Action<IHandlerConfigurable> config);
        IMicroserviceConfigurable UseDiscovery<TDiscovery>(Action<TDiscovery> config) where TDiscovery : IDiscovery;
    }
}