using System;
using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Storage
{
    public interface IPersistenceProvider<TData> where TData : class
    {
        Task<TData> Get(Guid correlationId);
        Task Save(TData obj);
        Task Remove(Guid correlationId);
    }
}
