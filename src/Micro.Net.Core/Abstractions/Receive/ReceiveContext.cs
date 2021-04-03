using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static ReceiveContext<TRequest, TResponse> Create(TRequest request, IDictionary<string, string[]> headers)
        {
            return new ReceiveContext<TRequest, TResponse>()
            {
                Source = new Uri("null://"),
                Destination = new Uri("null://"),
                Request = new RequestContext<TRequest>()
                {
                    Headers = new Dictionary<string, string[]>(headers),
                    Payload = request
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