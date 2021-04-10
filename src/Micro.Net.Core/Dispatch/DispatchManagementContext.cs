using System;
using System.Collections.Generic;
using Micro.Net.Abstractions;
using Micro.Net.Receive;

namespace Micro.Net.Dispatch
{
    public class DispatchManagementContext<TRequest, TResponse> : DispatchContext<TRequest,TResponse>, IDispatchManagementContext<TRequest,TResponse> where TRequest : IContract<TResponse>
    {
        public DispatchOptions Options { get; set; }

        public static DispatchManagementContext<TRequest, TResponse> Create(TRequest request)
        {
            DispatchManagementContext<TRequest, TResponse> context = new DispatchManagementContext<TRequest, TResponse>()
            {
                Options = DispatchOptions.Create(),
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
                    Payload = request
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