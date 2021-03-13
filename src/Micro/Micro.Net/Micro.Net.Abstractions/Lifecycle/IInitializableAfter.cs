using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Lifecycle
{
    public interface IInitializableAfter<TStep> where TStep : LifeCycleStep { Task Initialize(); }
}