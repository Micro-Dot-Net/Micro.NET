namespace Micro.Net.Abstractions.Storage
{
    public interface IPersistenceProviderFactory
    {
        IPersistenceProvider<TData> Create<TData>() where TData : class;
    }
}