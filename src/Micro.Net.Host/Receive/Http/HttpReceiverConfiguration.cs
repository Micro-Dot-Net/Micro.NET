using System;
using System.Collections.Generic;

namespace Micro.Net.Receive.Http
{
    public class HttpReceiverConfiguration
    {
        public string[] BaseUris { get; set; }
        public Dictionary<string, (Type, Type)> PathMaps { get; set; }
    }
}