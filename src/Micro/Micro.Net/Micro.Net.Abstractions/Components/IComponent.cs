using System.Threading;
using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Components
{
    public interface IComponent<TComponent> where TComponent : ComponentKind { Task Initialize(CancellationToken cancellationToken); Task Start(CancellationToken cancellationToken); Task Stop(CancellationToken cancellationToken); }
}