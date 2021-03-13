using System;
using System.Collections.Generic;

namespace Micro.Net.Host.Discovery.Abstractions
{
    public class SynMessage
    {
        public string ClusterId { get; set; }
        public SynFlags Flags { get; set; }
    }

    public enum SynFlags
    {
        IncludeHealth = 1,
    }

    public class AckMessage
    {
        public string NodeId { get; set; }
        public string ClusterId { get; set; }
        public Dictionary<SynFlags, object> Flags { get; set; }
    }

    public enum HealthCheckResult
    {
        Uncertain,
        Healthy,
        Unhealthy
    }

    public class AnnounceMessage
    {
        public string NodeId { get; set; }
        public string ClusterId { get; set; }
        public IDictionary<string, IDictionary<ServiceFlags, object>> Capabilities { get; set; }
    }

    public enum ServiceFlags
    {
        Transports
    }

    public class HelloMessage
    {
        public string NodeId { get; set; }
        public string ClusterId { get; set; }
    }

    public class GoodbyeMessage
    {
        public string NodeId { get; set; }
        public string ClusterId { get; set; }
    }
}
