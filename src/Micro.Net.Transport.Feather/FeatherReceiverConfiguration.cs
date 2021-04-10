using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Micro.Net.Transport.Feather
{
    public class FeatherReceiverConfiguration
    {
        public ICollection<string> BaseUris { get; set; }
        public Dictionary<(string path, HttpMethod verb), (Type, Type)> PathMaps { get; set; }
    }
}