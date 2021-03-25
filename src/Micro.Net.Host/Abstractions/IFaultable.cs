using System;

namespace Micro.Net
{
    public interface IFaultable
    {
        bool IsFaulted { get; }
        bool TryGetFault(out Exception ex);
        void SetFault(Exception ex);
    }
}