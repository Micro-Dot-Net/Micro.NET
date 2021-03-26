using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Sagas
{
    public interface ISagaTimeoutHandler<TTimeout> : IHandle<TTimeout> where TTimeout : ISagaTimeout
    {
        Task Handle(TTimeout timeout, ISagaContext context);
    }
}