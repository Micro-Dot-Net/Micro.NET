using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Lifecycle
{
    public interface IStartableAfter<TStep> where TStep : LifeCycleStep { Task Start(); }
}