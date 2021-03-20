using System;
using System.Threading;
using System.Threading.Tasks;
using Micro.Net.Abstractions.Messages;
using Micro.Net.Abstractions.Messages.Receive;

namespace Micro.Net.Host.MassTransit
{
    public class BusReceiverService : ReceiverService
    {
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

        public event Action<TransportEnvelope> OnReceive;
    }
}