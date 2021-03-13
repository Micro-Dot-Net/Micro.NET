using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Lifecycle
{
    public interface IStoppableAfter<TStep> where TStep : LifeCycleStep { Task Stop(); }
}