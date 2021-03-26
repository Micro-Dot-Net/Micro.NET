using System.Threading;
using System.Threading.Tasks;

namespace Micro.Net.Abstractions
{
    public interface IStartable
    {
        Task Start(CancellationToken cancellationToken);
    }
}