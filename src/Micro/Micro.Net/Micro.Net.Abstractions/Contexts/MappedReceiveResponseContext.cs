using System.Collections.Generic;

namespace Micro.Net.Abstractions.Contexts
{
    public class MappedReceiveResponseContext<TResponse>
    {
        public IDictionary<string, string> Headers { get; set; }
        public TResponse Body { get; set; }
    }
}