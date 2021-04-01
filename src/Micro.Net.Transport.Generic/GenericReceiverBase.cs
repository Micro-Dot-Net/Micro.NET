using System;
using System.Threading;
using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Micro.Net.Receive;

namespace Micro.Net.Transport.Generic
{
    public abstract class GenericReceiverBase : IStartable, IStoppable
    {
        private readonly IReceivePipeFactory _receivePipeFactory;

        protected GenericReceiverBase(IReceivePipeFactory receivePipeFactory)
        {
            _receivePipeFactory = receivePipeFactory;
        }

        public virtual async Task Start(CancellationToken cancellationToken){ }

        public virtual async Task Stop(CancellationToken cancellationToken) { }

        /// <summary>
        /// Used to pipe received messages into the core.
        /// </summary>
        /// <typeparam name="TRequest">Request type received.</typeparam>
        /// <typeparam name="TResponse">Response type expected to go back. If no response is expected, use <see cref="ValueTuple"/> for this parameter.</typeparam>
        /// <param name="context">Constructed object containing information relevant to processing.</param>
        /// <returns>Awaitable task that indicates processing has completed. Check <see cref="ContextStatus"/> on the passed <see cref="ReceiveContext{TRequest,TResponse}"/> for completion status.</returns>
        protected async Task Dispatch<TRequest, TResponse>(ReceiveContext<TRequest,TResponse> context)
        {
            ReceiveContextDelegate<TRequest,TResponse> del = _receivePipeFactory.Create<TRequest, TResponse>();

            await del.Invoke(context);
        }
    }
}
