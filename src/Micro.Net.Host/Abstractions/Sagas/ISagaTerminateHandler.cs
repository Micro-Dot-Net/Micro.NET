using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Sagas
{
    public interface ISagaTerminateHandler<TData>
    {
        Task HandleTerminate(SagaTerminateContext<TData> context);
    }

    public interface ISagaTerminateHandler
    {
        Task HandleTerminate<TData>(TData data, SagaTerminateContext context);
    }
}