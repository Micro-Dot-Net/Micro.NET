using System.Threading.Tasks;
using Micro.Net.Handling;

namespace Micro.Net.Abstractions.Sagas
{
    public interface ISagaNotFoundHandler
    {
        Task Handle(object message, IHandlerContext context);
    }
}