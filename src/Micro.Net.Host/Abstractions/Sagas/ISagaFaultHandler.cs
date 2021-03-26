using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Sagas
{

    public interface ISagaFaultHandler
    {
        Task HandleFault(object message, object data, SagaFaultContext context);
    }
}