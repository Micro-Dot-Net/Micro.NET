using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Micro.Net.Exceptions;

namespace Micro.Net.Dispatch
{
    public class DispatchManager<TRequest, TResponse> : IDispatchManager<TRequest,TResponse> where TRequest : IContract<TResponse>
    {
        private readonly IEnumerable<IDispatcher> _dispatchers;

        public DispatchManager(IEnumerable<IDispatcher> dispatchers)
        {
            _dispatchers = dispatchers;
        }

        public async Task Handle(IDispatchManagementContext<TRequest, TResponse> request, CancellationToken cancellationToken)
        {
            IDispatcher dispatcher = null;

            foreach (IDispatcher candidateDispatcher in _dispatchers)
            {
                if (candidateDispatcher.Available.Contains((typeof(TRequest), typeof(TResponse))))
                {
                    //TODO: Check features against options
                    dispatcher = candidateDispatcher;
                    break;
                }
            }

            if (dispatcher == null)
            {
                request.SetFault(MicroDispatcherException.NoMapping(typeof(TRequest), typeof(TResponse)));
            }
            else
            {
                try
                {
                    await dispatcher.Handle<TRequest, TResponse>(request);

                    request.SetResolve();
                }
                catch (Exception ex)
                {
                    request.SetFault(ex);
                }
            }
        }

        public async Task<ValueTuple> Handle(IDispatchManagementContext<TRequest, TResponse> request)
        {
            await Handle(request, CancellationToken.None);

            return ValueTuple.Create();
        }
    }
}