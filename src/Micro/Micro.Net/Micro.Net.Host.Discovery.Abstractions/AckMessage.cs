using System.Collections.Generic;

namespace Micro.Net.Host.Discovery.Abstractions
{
    public class AckMessage
    {
        public string NodeId { get; set; }
        public string ClusterId { get; set; }
        public Dictionary<SynFlags, object> Flags { get; set; }
    }
}