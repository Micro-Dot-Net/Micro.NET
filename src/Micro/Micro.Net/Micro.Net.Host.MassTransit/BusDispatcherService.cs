using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Micro.Net.Abstractions.Messages.Dispatch;

namespace Micro.Net.Host.MassTransit
{
    public class BusDispatcherService : DispatcherService
    {
        public async Task<TResponse> Dispatch<TRequest, TResponse>(TRequest message)
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

        public async Task<TResponse> Dispatch<TRequest, TResponse>(TRequest message, Action<DispatchOptions> opts)
        {
            throw new NotImplementedException();
        }

        public async Task Dispatch<TMessage>(TMessage message, Action<DispatchOptions> opts)
        {
            throw new NotImplementedException();
        }

        public bool CanHandle<TRequest, TResponse>()
        {
            throw new NotImplementedException();
        }

        public bool CanHandle<TMessage>()
        {
            throw new NotImplementedException();
        }
    }
}