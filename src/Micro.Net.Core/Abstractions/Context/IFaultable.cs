using System;

namespace Micro.Net.Abstractions
{
    public interface IFaultable
    {
        bool IsFaulted { get; }
        bool TryGetFault(out Exception ex);
        void SetFault(Exception ex);
    }
}