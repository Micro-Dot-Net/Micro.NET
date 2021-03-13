using System;
using System.Collections.Generic;

namespace Micro.Net.Abstractions.Messages
{
    public class TransportMessage
    {
        public Uri Origin { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public byte[] Body { get; set; }
    }
}