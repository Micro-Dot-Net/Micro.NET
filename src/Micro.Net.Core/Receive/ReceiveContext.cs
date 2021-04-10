using System;
using System.Collections.Generic;
using System.Diagnostics;
using Micro.Net.Abstractions;

namespace Micro.Net.Receive
{
    public class ReceiveContext<TRequest, TResponse> : ContextBase, IReceiveContext<TRequest, TResponse>
    {
        public Uri Source { get; set; }
        public Uri Destination { get; set; }
        public IRequestContext<TRequest> Request { get; set; }
        public IResponseContext<TResponse> Response { get; set; }

    }

    public static class ReceiveContext
    {
        public static ReceiveContext<TRequest, TResponse> Create<TRequest, TResponse>()
        {
            return new ReceiveContext<TRequest, TResponse>()
            {
                Source = new Uri("null://"),
                Destination = new Uri("null://"),
                Request = new RequestContext<TRequest>()
                {
                    Headers = new Dictionary<string, string[]>(),
                    Payload = default
                },
                Response = new ResponseContext<TResponse>()
                {
                    Headers = new Dictionary<string, string[]>(),
                    Payload = default
                },
                Status = ContextStatus.Live
            };
        }
    }
}