using System.Threading.Tasks;

namespace Micro.Net.Host.Abstractions.Sagas
{
    public interface ISagaNotFoundHandler
    {
        Task Handle(object message, IHandlerContext context);
    }
}