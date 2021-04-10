using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Micro.Net.Dispatch;

namespace Micro.Net.Transport.Generic
{
    public abstract class GenericDispatcherBase : IDispatcher, IStartable, IStoppable
    {
        /// <summary>
        /// Advertises features this dispatcher supports.
        /// </summary>
        public abstract ISet<DispatcherFeature> Features { get; }

        /// <summary>
        /// Advertises message types this dispatcher is capable of handling.
        /// </summary>
        public abstract IEnumerable<(Type, Type)> Available { get; }

        /// <summary>
        /// Called by upstream core components when a message is ready to be dispatched.
        /// </summary>
        /// <typeparam name="TRequest">Type of message to be dispatched.</typeparam>
        /// <typeparam name="TResponse">Type of response expected. When no response is expected, <see cref="ValueTuple"/> is used to indicate no value is expected.</typeparam>
        /// <param name="messageContext">Message context to be dispatched. Value for dispatching is <see>
        ///         <cref>messageContext.Request.Payload</cref>
        ///     </see>, response should be added to <see><crf>messageContext.Response.Payload</crf></see>
        /// </param>
        /// <returns></returns>
        public abstract Task Handle<TRequest, TResponse>(IDispatchContext<TRequest,TResponse> messageContext) where TRequest : IContract<TResponse>;

        public virtual async Task Start(CancellationToken cancellationToken) { }

        public virtual async Task Stop(CancellationToken cancellationToken) { }
    }
}