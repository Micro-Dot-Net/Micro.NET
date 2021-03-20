using System.Collections.Generic;

namespace Micro.Net.Host.Discovery.Abstractions
{
    public class AnnounceMessage
    {
        public string NodeId { get; set; }
        public string ClusterId { get; set; }
        public IDictionary<string, IDictionary<ServiceFlags, object>> Capabilities { get; set; }
    }
}