using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Sagas
{
    public interface ISagaContext : IFaultable, IResolvable, ITerminable
    {
        Task DispatchTimeout<TTimeout>() where TTimeout : ISagaTimeout;
    }
}