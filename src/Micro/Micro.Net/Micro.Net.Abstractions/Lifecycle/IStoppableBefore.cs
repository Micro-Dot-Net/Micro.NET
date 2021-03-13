using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Lifecycle
{
    public interface IStoppableBefore<TStep> where TStep : LifeCycleStep { Task Stop(); }
}