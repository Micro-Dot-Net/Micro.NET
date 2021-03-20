using System.Threading;
using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Lifecycle
{
    public interface IRun<TStep> where TStep : LifeCycleStep
    {
        Task Run(RunContext<TStep> step, CancellationToken cancellationToken);
    }
}