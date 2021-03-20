using System;

namespace Micro.Net.Host.Discovery.Configuration
{
    public class ServiceEntry
    {
        public Uri Address { get; set; }
        public string Contract { get; set; }
        public string Assembly { get; set; }
    }
}