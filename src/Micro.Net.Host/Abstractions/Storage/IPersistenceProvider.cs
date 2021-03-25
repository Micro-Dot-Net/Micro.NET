using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Net.Host.Abstractions.Storage
{
    public interface IPersistenceProvider<TData> where TData : class
    {
        Task<TData> Get(Guid correlationId);
        Task Save(TData obj);
        Task Remove(Guid correlationId);
    }
}
