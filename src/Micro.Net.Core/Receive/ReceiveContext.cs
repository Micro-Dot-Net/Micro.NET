using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Transport;

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
        public static IReceiveContext<TRequest, TResponse> Create<TRequest, TResponse>()
        {
            return Create<TRequest, TResponse>(null);
        }

        public static IReceiveContext<TRequest, TResponse> Create<TRequest, TResponse>(Envelope<TRequest> envelope)
        {
            return new ReceiveContext<TRequest, TResponse>()
            {
                Source = new Uri("null://"),
                Destination = new Uri("null://"),
                Request = new RequestContext<TRequest>()
                {
                    Headers = envelope?.Headers.ToDictionary(x => x.Key, y => y.Value) ?? new Dictionary<string, string[]>(),
                    Payload = envelope != null ? envelope.Message ?? default : default
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