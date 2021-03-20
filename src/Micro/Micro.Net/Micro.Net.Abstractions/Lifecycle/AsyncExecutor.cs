using System.Threading;
using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Lifecycle
{
    public delegate Task AsyncExecutor(CancellationToken token);
}