using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Micro.Net.Dispatch.Http
{
    public class HttpDispatcherConfiguration
    {
        public IDictionary<(Type,Type), (Uri, HttpMethod)> Routes { get; set; }
        public IDictionary<string, string[]> DefaultHeaders { get; set; }
    }
}