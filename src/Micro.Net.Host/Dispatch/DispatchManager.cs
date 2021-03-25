using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Micro.Net.Host.Dispatch
{
    public class DispatchManager<TRequest, TResponse> : IDispatchManager<TRequest,TResponse> where TRequest : IContract<TResponse>
    {
        private readonly IEnumerable<IDispatcher> _dispatchers;

        public DispatchManager(IEnumerable<IDispatcher> dispatchers)
        {
            _dispatchers = dispatchers;
        }

        public async Task<Unit> Handle(DispatchContext<TRequest, TResponse> request, CancellationToken cancellationToken)
        {
            IDispatcher dispatcher = null;

            foreach (IDispatcher candidateDispatcher in _dispatchers)
            {
                if (candidateDispatcher.Available.Contains((typeof(TRequest), typeof(TResponse))))
                {
                    dispatcher = candidateDispatcher;
                    break;
                }
            }

            if (dispatcher == null)
            {
                request.SetFault(MicroDispatcherException.NoMapping);
            }
            else
            {
                try
                {
                    request.Response.Payload =
                        await dispatcher.Handle<TRequest, TResponse>(request.Request.Payload, request.Options);

                    request.SetResolve();
                }
                catch (Exception ex)
                {
                    request.SetFault(ex);
                }
            }

            return Unit.Value;
        }
    }
}