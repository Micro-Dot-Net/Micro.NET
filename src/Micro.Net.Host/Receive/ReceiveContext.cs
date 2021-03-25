using System;
using Automatonymous;
using MediatR;

namespace Micro.Net
{
    public class ReceiveContext<TRequest, TResponse> : ContextBase, IRequest
    {
        public Uri Source { get; set; }
        public Uri Destination { get; set; }
        public RequestContext<TRequest> Request { get; set; }
        public ResponseContext<TResponse> Response { get; set; }
    }
}