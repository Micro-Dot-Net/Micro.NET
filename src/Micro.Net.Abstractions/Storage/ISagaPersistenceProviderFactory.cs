using Micro.Net.Abstractions.Sagas;

namespace Micro.Net.Abstractions.Storage
{
    public interface ISagaPersistenceProviderFactory
    {
        ISagaPersistenceProvider<TData> Create<TData>() where TData : class, ISagaData;
    }
}