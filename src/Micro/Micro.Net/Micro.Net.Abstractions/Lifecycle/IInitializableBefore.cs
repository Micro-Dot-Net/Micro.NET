using System.Threading;
using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Lifecycle
{
    public interface IInitializableBefore<TStep> where TStep : LifeCycleStep { Task Initialize(CancellationToken cancellationToken); }
}