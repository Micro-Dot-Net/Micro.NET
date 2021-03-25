using System.Collections.Generic;
using System.Threading.Tasks;

namespace Micro.Net
{
    public interface ITerminable
    {
        bool IsTerminated { get; }
        bool TryGetTerminate(out string reason, out IDictionary<string, string> auxData);
        void SetTerminate(string reason, IDictionary<string, string> auxData = null);
    }
}