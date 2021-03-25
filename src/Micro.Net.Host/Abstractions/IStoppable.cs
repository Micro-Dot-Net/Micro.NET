using System.Threading;
using System.Threading.Tasks;

namespace Micro.Net
{
    public interface IStoppable
    {
        Task Stop(CancellationToken cancellationToken);
    }
}