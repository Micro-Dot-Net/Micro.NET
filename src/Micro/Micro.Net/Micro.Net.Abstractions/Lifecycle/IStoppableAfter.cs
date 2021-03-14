using System.Threading;
using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Lifecycle
{
    public interface IStoppableAfter<TStep> where TStep : LifeCycleStep { Task Stop(CancellationToken cancellationToken); }
}