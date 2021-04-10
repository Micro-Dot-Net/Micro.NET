using System.Threading;
using System.Threading.Tasks;
using Micro.Net.Core.Abstractions.Management;

namespace Micro.Net.Abstractions
{
    public interface IStoppable : IMicroComponent
    {
        Task Stop(CancellationToken cancellationToken);
    }
}