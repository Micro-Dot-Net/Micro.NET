using System;
using MediatR;
using Micro.Net.Abstractions;

namespace Micro.Net.Receive
{
    public class ReceiveContext<TRequest, TResponse> : ContextBase, IRequest
    {
        public Uri Source { get; set; }
        public Uri Destination { get; set; }
        public RequestContext<TRequest> Request { get; set; }
        public ResponseContext<TResponse> Response { get; set; }
    }
}