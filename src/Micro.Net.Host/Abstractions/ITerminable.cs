using System.Collections.Generic;

namespace Micro.Net.Abstractions
{
    public interface ITerminable
    {
        bool IsTerminated { get; }
        bool TryGetTerminate(out string reason, out IDictionary<string, string> auxData);
        void SetTerminate(string reason, IDictionary<string, string> auxData = null);
    }
}