using System.Collections.Generic;

namespace Micro.Net.Receive
{
    public class ResponseContext<TResponse> : IResponseContext<TResponse>
    {
        public Dictionary<string, string[]> Headers { get; set; }
        public TResponse Payload { get; set; }
    }
}