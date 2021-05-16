using System;
using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Timeout;
using Micro.Net.Dispatch;

namespace Micro.Net.Handling
{
    public interface IHandlerContext
    {
        Task<TResponse> Dispatch<TRequest, TResponse>(TRequest request, Action<DispatchOptions> ctxAction = null, bool? throwOnFault = null) where TRequest : IContract<TResponse>;
        Task Dispatch<TRequest>(TRequest request, Action<DispatchOptions> ctxAction = null, bool? throwOnFault = null) where TRequest : IContract;

        Task RequestTimeout<TTimeout>(TTimeout timeout, Action<TimeoutOptions> ctxAction = null) where TTimeout : ITimeout;
    }
}