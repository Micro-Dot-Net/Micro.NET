using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Sagas
{
    public interface ISagaTerminateHandler<TData>
    {
        Task HandleTerminate(ISagaTerminateContext<TData> context);
    }

    public interface ISagaTerminateHandler
    {
        Task HandleTerminate<TData>(TData data, ISagaTerminateContext context);
    }
}