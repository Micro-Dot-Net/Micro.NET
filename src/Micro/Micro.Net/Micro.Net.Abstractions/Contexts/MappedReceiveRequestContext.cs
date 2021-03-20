using System;
using System.Collections.Generic;

namespace Micro.Net.Abstractions.Contexts
{
    public class MappedReceiveRequestContext<TRequest>
    {
        public Uri Origin { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public TRequest Body { get; set; }
    }
}