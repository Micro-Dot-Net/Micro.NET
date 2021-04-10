using System;
using Micro.Net.Abstractions;

namespace Micro.Net.Receive
{
    public interface IReceiveContext<TRequest, TResponse> : IContextBase
    {
        Uri Source { get; set; }
        Uri Destination { get; set; }
        IRequestContext<TRequest> Request { get; set; }
        IResponseContext<TResponse> Response { get; set; }
    }
}