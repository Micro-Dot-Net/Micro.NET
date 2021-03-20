using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Micro.Net.Abstractions.Contexts
{
    public abstract class ReceiveResponseContext
    {
        public IDictionary<string, string> Headers { get; set; }
        public JObject Body { get; set; }
    }
}