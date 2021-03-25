using System.Threading;
using System.Threading.Tasks;

namespace Micro.Net
{
    public interface IStartable
    {
        Task Start(CancellationToken cancellationToken);
    }
}