using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Sagas
{
    public interface ISagaStepHandler<TMessage> where TMessage : ISagaContract
    {
        Task Handle(TMessage message, ISagaContext context);
    }
}