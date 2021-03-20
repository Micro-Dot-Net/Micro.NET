using System;
using Micro.Net.Abstractions.Discovery;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Net.Abstractions.Configuration
{
    public interface IMicroserviceConfigurable
    {
        IMicroserviceConfigurable ConfigureReceivers(Action<IReceiverConfigurable> config);
        IMicroserviceConfigurable ConfigureDispatchers(Action<IDispatcherConfigurable> config);
        IMicroserviceConfigurable ConfigureHandlers(Action<IHandlerConfigurable> config);
        IMicroserviceConfigurable UseDiscovery<TDiscovery>(Action<TDiscovery> config) where TDiscovery : IDiscovery, new();
        IMicroserviceConfigurable ConfigureContainer(Action<IServiceCollection, IConfiguration> config);
    }
}