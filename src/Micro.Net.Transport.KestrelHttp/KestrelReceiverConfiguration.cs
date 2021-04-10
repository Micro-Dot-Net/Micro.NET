using System;
using System.Collections.Generic;

namespace Micro.Net.Transport.KestrelHttp
{
    public class KestrelReceiverConfiguration
    {
        public ICollection<string> BaseUris { get; set; }
        public Dictionary<string, (Type, Type)> PathMaps { get; set; }
    }
}