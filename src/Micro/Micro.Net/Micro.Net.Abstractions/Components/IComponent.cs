using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Components
{
    public interface IComponent<TComponent> where TComponent : ComponentKind { Task Initialize(); Task Start(); Task Stop(); }
}