using System;
using System.Collections.Generic;

namespace Micro.Net.Abstractions.Sagas
{
    public interface ISagaFinderContext : IFaultable, IResolvable, ITerminable
    {

    }

    public class SagaFinderContext : ISagaFinderContext
    {
        public bool IsFaulted { get; }
        public bool TryGetFault(out Exception ex)
        {
            throw new NotImplementedException();
        }

        public void SetFault(Exception ex)
        {
            throw new NotImplementedException();
        }

        public bool IsResolved { get; }
        public void SetResolve()
        {
            throw new NotImplementedException();
        }

        public bool IsTerminated { get; }
        public bool TryGetTerminate(out string reason, out IDictionary<string, string> auxData)
        {
            throw new NotImplementedException();
        }

        public void SetTerminate(string reason, IDictionary<string, string> auxData = null)
        {
            throw new NotImplementedException();
        }
    }
}