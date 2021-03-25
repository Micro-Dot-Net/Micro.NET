using System.Collections.Generic;

namespace Micro.Net
{
    public class RequestContext<TRequest>
    {
        public Dictionary<string,string> Headers { get; set; }
        public TRequest Payload { get; set; }
    }
}