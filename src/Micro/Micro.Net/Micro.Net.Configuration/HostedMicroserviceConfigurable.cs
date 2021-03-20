using System;
using System.Collections.Generic;
using Micro.Net.Abstractions.Configuration;
using Micro.Net.Abstractions.Discovery;
using Micro.Net.Abstractions.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Net.Configuration
{
    public class HostedMicroserviceConfigurable : IMicroserviceConfigurable
    {
        private ICollection<Action<IServiceCollection, IConfiguration>> containerConfigs =
            new List<Action<IServiceCollection, IConfiguration>>();

        private ReceiverConfigurable receiverConfig = default;

        private DispatcherConfigurable dispatcherConfig = default;

        private HandlerConfigurable handlerConfig = default;

        private IDiscovery discoveryConfig = null;

        public IMicroserviceConfigurable ConfigureReceivers(Action<IReceiverConfigurable> config)
        {
            if (receiverConfig != null)
            {
                throw MicroConfigurationException.AlreadyConfigured(nameof(ConfigureReceivers));
            }

            receiverConfig = new ReceiverConfigurable();

            config(receiverConfig);

            return this;
        }

        public IMicroserviceConfigurable ConfigureDispatchers(Action<IDispatcherConfigurable> config)
        {
            if (dispatcherConfig != null)
            {
                throw MicroConfigurationException.AlreadyConfigured(nameof(ConfigureDispatchers));
            }

            dispatcherConfig = new DispatcherConfigurable();

            config(dispatcherConfig);

            return this;
        }

        public IMicroserviceConfigurable ConfigureHandlers(Action<IHandlerConfigurable> config)
        {
            if (handlerConfig != null)
            {
                throw MicroConfigurationException.AlreadyConfigured(nameof(ConfigureHandlers));
            }

            handlerConfig = new HandlerConfigurable();

            config(handlerConfig);

            return this;
        }

        public IMicroserviceConfigurable UseDiscovery<TDiscovery>(Action<TDiscovery> config) where TDiscovery : IDiscovery, new()
        {
            if (discoveryConfig != null)
            {
                throw MicroConfigurationException.AlreadyConfigured(nameof(UseDiscovery));
            }

            TDiscovery _discoveryConfig = new TDiscovery();

            config(_discoveryConfig);

            discoveryConfig = _discoveryConfig;

            return this;
        }

        public IMicroserviceConfigurable ConfigureContainer(Action<IServiceCollection, IConfiguration> config)
        {
            containerConfigs.Add(config);

            return this;
        }

        public void ConfigureContainer(IServiceCollection services, IConfiguration config)
        {

        }
    }
}