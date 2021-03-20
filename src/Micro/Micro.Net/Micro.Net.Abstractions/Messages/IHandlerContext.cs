using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Micro.Net.Abstractions.Messages.Dispatch;

namespace Micro.Net.Abstractions.Messages
{
    public interface IHandlerContext
    {
        public IRequestContext Request { get; }
        public IResponseContext Response { get; }
        Task<TResponse> Dispatch<TRequest, TResponse>(TRequest request, Action<DispatchOptions> ctxAction = null) where TRequest : IContract<TResponse>;
        Task Dispatch<TRequest>(TRequest request, Action<DispatchOptions> ctxAction = null) where TRequest : IContract;
    }
}