using System;
using System.Collections.Generic;
using Micro.Net.Abstractions;
using Micro.Net.Receive;

namespace Micro.Net.Dispatch
{
    public class DispatchContext<TRequest,TResponse> : ContextBase, IDispatchContext<TRequest, TResponse> where TRequest : IContract<TResponse>
    {
        public Uri Source { get; set; }
        public Uri Destination { get; set; }
        public IRequestContext<TRequest> Request { get; set; }
        public IResponseContext<TResponse> Response { get; set; }
    }

    public static class DispatchContext
    {
        public static DispatchContext<TRequest, TResponse> Create<TRequest,TResponse>() where TRequest : IContract<TResponse>
        {
            DispatchContext<TRequest, TResponse> context = new DispatchContext<TRequest, TResponse>()
            {
                Destination = new Uri("null://"),
                Source = new Uri("null://"),
                Request = new RequestContext<TRequest>()
                {
                    Headers = new Dictionary<string, string[]>()
                    {
                        { "X-RequestType", new []
                            {
                                typeof(TRequest).AssemblyQualifiedName
                            }
                        },
                    },
                    Payload = default
                },
                Response = new ResponseContext<TResponse>()
                {
                    Headers = new Dictionary<string, string[]>(),
                    Payload = default
                }
            };

            if (typeof(TResponse) != typeof(ValueTuple))
            {
                context.Request.Headers.Add("X-ResponseType", new[] { typeof(TResponse).AssemblyQualifiedName });
            }

            return context;
        }
    }
}
