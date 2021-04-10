using System;
using Micro.Net.Abstractions;
using Micro.Net.Receive;

namespace Micro.Net.Dispatch
{
    public interface IDispatchContext<TRequest, TResponse> : IContextBase where TRequest : IContract<TResponse>
    {
        Uri Source { get; set; }
        Uri Destination { get; set; }
        IRequestContext<TRequest> Request { get; set; }
        IResponseContext<TResponse> Response { get; set; }
    }
}