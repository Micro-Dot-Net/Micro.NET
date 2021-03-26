using System.Threading;
using System.Threading.Tasks;

namespace Micro.Net.Abstractions
{
    public interface IStoppable
    {
        Task Stop(CancellationToken cancellationToken);
    }
}