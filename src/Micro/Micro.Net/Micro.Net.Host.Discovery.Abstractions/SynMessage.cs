using System;

namespace Micro.Net.Host.Discovery.Abstractions
{
    public class SynMessage
    {
        public string ClusterId { get; set; }
        public SynFlags Flags { get; set; }
    }
}
