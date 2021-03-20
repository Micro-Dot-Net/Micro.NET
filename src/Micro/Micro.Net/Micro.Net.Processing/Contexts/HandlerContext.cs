using System;
using System.Threading.Tasks;
using Micro.Net.Abstractions.Messages;
using Micro.Net.Abstractions.Messages.Dispatch;

namespace Micro.Net.Processing.Contexts
{
    public class HandlerContext : IHandlerContext
    {
        public IRequestContext Request { get; }
        public IResponseContext Response { get; }
        
        public async Task<TResponse> Dispatch<TRequest, TResponse>(TRequest request, Action<DispatchOptions> ctxAction = null) where TRequest : IContract<TResponse>
        {
            throw new NotImplementedException();
        }

        public async Task Dispatch<TRequest>(TRequest request, Action<DispatchOptions> ctxAction = null) where TRequest : IContract
        {
            throw new NotImplementedException();
        }
    }
}