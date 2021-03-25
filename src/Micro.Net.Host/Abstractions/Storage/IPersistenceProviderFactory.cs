namespace Micro.Net.Host.Abstractions.Storage
{
    public interface IPersistenceProviderFactory
    {
        IPersistenceProvider<TData> Create<TData>() where TData : class;
    }
}