using System;
using System.Collections.Generic;

namespace Micro.Net.Host.Discovery.Configuration
{
    public class ServiceEntryEqualityComparer : IEqualityComparer<ServiceEntry>
    {
        public static IEqualityComparer<ServiceEntry> Default => new ServiceEntryEqualityComparer();

        public bool Equals(ServiceEntry x, ServiceEntry y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return Equals(x.Address, y.Address) && x.Contract == y.Contract && x.Assembly == y.Assembly;
        }

        public int GetHashCode(ServiceEntry obj)
        {
            return HashCode.Combine(obj.Address, obj.Contract, obj.Assembly);
        }
    }
}