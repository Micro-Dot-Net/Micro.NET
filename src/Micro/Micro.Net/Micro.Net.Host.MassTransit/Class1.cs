using System;
using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Configuration;
using Micro.Net.Abstractions.Messages.Dispatch;
using Micro.Net.Abstractions.Messages.Receive;

namespace Micro.Net.Host.MassTransit
{
    public class BusReceiver : IReceiverConfiguration
    {
    }

    public class BusDispatcher : IDispatcherConfiguration
    {

    }

    public class BusReceiverService : ReceiverService
    {

    }

    public class BusDispatcherService : DispatcherService
    {
        public async Task<TResponse> Dispatch<TRequest, TResponse>(TRequest message)
        {
            throw new NotImplementedException();
        }
    }
}
