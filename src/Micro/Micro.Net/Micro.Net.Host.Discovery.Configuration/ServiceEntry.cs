using System;
using System.Collections.Generic;

namespace Micro.Net.Host.Discovery.Configuration
{
    public class ServiceEntry
    {
        public Uri Address { get; set; }
        public string Contract { get; set; }
        public string Assembly { get; set; }
    }

    public class ServiceEntryEqualityComparer : IEqualityComparer<ServiceEntry>
    {
        

        public static IEqualityComparer<ServiceEntry> Default => new ServiceEntryEqualityComparer();
    }
}