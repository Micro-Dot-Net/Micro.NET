using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Micro.Net.Abstractions.Messages
{
    public class TransportMessage
    {
        public Uri Origin { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public JObject Body { get; set; }
    }
}