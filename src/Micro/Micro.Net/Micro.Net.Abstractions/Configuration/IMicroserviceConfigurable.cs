using System;

namespace Micro.Net.Abstractions.Configuration
{
    public interface IMicroserviceConfigurable
    {
        IMicroserviceConfigurable ConfigureReceivers(Action<IReceiverConfigurable> config);
        IMicroserviceConfigurable ConfigureDispatchers(Action<IDispatcherConfigurable> config);
        IMicroserviceConfigurable ConfigureHandlers(Action<IHandlerConfigurable> config);
    }
}