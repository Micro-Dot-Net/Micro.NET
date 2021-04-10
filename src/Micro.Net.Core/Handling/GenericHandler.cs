using System;
using System.Threading.Tasks;
using Micro.Net.Abstractions;

namespace Micro.Net.Handling
{
    public class GenericHandler<TRequest, TResponse> : IHandle<TRequest,TResponse> where TRequest : IContract<TResponse>
    {
        private readonly Func<TRequest, IHandlerContext, Task<TResponse>> _predicate;

        public GenericHandler(Func<TRequest, IHandlerContext, Task<TResponse>> predicate)
        {
            _predicate = predicate;
        }

        public async Task<TResponse> Handle(TRequest message, IHandlerContext context)
        {
            return await _predicate(message,context);
        }
    }
}