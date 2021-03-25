using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Micro.Net.Host.Dispatch
{
    public class DispatchContext<TRequest,TResponse> : ContextBase, IRequest where TRequest : IContract<TResponse>
    {
        public Uri Source { get; set; }
        public Uri Destination { get; set; }
        public RequestContext<TRequest> Request { get; set; }
        public ResponseContext<TResponse> Response { get; set; }
        public DispatchOptions Options { get; set; }

        public static DispatchContext<TRequest, TResponse> Create(TRequest request)
        {
            DispatchContext<TRequest, TResponse> context = new DispatchContext<TRequest, TResponse>()
            {
                Options = DispatchOptions.Create(),
                Destination = new Uri("null://"),
                Source = new Uri("null://"),
                Request = new RequestContext<TRequest>()
                {
                    Headers = new Dictionary<string, string>()
                    {
                        {"X-RequestType", typeof(TRequest).AssemblyQualifiedName },
                    },
                    Payload = request
                },
                Response = new ResponseContext<TResponse>()
                {
                    Headers = new Dictionary<string, string>(),
                    Payload = default
                }
            };

            if (typeof(TResponse) != typeof(ValueTuple))
            {
                context.Request.Headers.Add("X-ResponseType", typeof(TResponse).AssemblyQualifiedName);
            }

            return context;
        }
    }
}
