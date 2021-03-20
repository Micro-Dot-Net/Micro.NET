using Newtonsoft.Json.Linq;

namespace Micro.Net.Abstractions.Contexts
{
    public class MappedReceiveContext<TRequest, TResponse>
    {
        public MappedReceiveRequestContext<TRequest> Request { get; set; }
        public MappedReceiveResponseContext<TResponse> Response { get; set; }
    }
}