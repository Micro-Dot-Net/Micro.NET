using System.Threading.Tasks;
using Micro.Net.Abstractions.Sagas;

namespace Micro.Net.Abstractions.Storage
{
    public interface ISagaPersistenceProvider<TData> : IPersistenceProvider<TData> where TData : class, ISagaData
    {
        Task<TData> Get(SagaKey key);
        Task Save(TData obj);
        Task Complete(SagaKey key);
    }
}