using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Micro.Net.Abstractions.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Micro.Net.Storage.Sql
{
    public class SqlStorageProvider<TData> : IPersistenceProvider<TData> where TData : class
    {
        private readonly DbSet<TData> _set;
        private readonly DbContext _context;

        internal SqlStorageProvider(DbSet<TData> set, DbContext context)
        {
            _set = set;
            _context = context;
        }
        
        public async Task<TData> Get(Guid correlationId)
        {
            throw new NotImplementedException();
        }

        public async Task Save(TData obj)
        {
            throw new NotImplementedException();
        }

        public async Task Remove(Guid correlationId)
        {
            throw new NotImplementedException();
        }
    }
}
