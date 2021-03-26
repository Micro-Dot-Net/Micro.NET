using System;
using System.Collections.Generic;

namespace Micro.Net.Abstractions
{
    public abstract class ContextBase : IFaultable, ITerminable, IResolvable
    {
        public ContextStatus Status { get; protected set; }
        private object _statusInfo;
        public bool IsFaulted => Status == ContextStatus.Faulted;
        public bool TryGetFault(out Exception ex)
        {
            if (!IsFaulted)
            {
                ex = default;
                return false;
            }

            ex = (Exception) _statusInfo;

            return true;
        }

        public void SetFault(Exception ex)
        {
            _setStatus(ContextStatus.Faulted);
            _statusInfo = ex;
        }

        public bool IsTerminated => Status == ContextStatus.Terminated;
        public bool TryGetTerminate(out string reason, out IDictionary<string, string> auxData)
        {
            if (!IsTerminated)
            {
                reason = default;
                auxData = default;
                return false;
            }

            (reason, auxData) = (ValueTuple<string,IDictionary<string, string>>)_statusInfo;

            return true;
        }

        public void SetTerminate(string reason, IDictionary<string, string> auxData = null)
        {
            _setStatus(ContextStatus.Terminated);
            _statusInfo = ValueTuple.Create(reason, auxData);
        }

        public bool IsResolved => Status == ContextStatus.Resolved;
        public void SetResolve()
        {
            _setStatus(ContextStatus.Resolved);
        }

        private bool _setStatus(ContextStatus status)
        {
            if (Status != ContextStatus.Live)
            {
                throw new InvalidStateException();
            }

            Status = status;

            return true;
        }
    }
}