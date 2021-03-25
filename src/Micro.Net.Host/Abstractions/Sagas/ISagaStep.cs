using System.Threading.Tasks;

namespace Micro.Net.Host.Abstractions.Sagas
{
    public interface ISagaStepHandler<TMessage> where TMessage : ISagaContract
    {
        Task Handle(TMessage message, ISagaContext context);
    }
}