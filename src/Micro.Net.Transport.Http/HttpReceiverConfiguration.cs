using System;
using System.Collections.Generic;

namespace Micro.Net.Transport.Http
{
    public class HttpReceiverConfiguration
    {
        public ICollection<string> BaseUris { get; set; }
        public Dictionary<string, (Type, Type)> PathMaps { get; set; }
    }
}