using System.Threading.Tasks;

namespace Micro.Net.Host.Abstractions.Sagas
{
    public interface ISagaTimeoutHandler<TTimeout> : IHandle<TTimeout> where TTimeout : ISagaTimeout
    {
        Task Handle(TTimeout timeout, ISagaContext context);
    }
}