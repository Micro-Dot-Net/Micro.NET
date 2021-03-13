using System;
using System.Threading.Tasks;
using Micro.Net.Abstractions.Components;

namespace Micro.Net.Abstractions.Messages.Dispatch
{
    public interface DispatcherService : IComponent<ComponentKind.Transport.Dispatch>
    {
        Task<TResponse> Dispatch<TRequest, TResponse>(TRequest message, Action<DispatchOptions> opts);
        Task Dispatch<TMessage>(TMessage message, Action<DispatchOptions> opts);

        bool CanHandle<TRequest, TResponse>();
        bool CanHandle<TMessage>();
    }
}