using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Micro.Net.Storage.Sql
{
    public class SqlStorageProvider<TData> : IStorageProvider<TData> where TData : class
    {
        private readonly DbSet<TData> _set;
        private readonly DbContext _context;

        internal SqlStorageProvider(DbSet<TData> set, DbContext context)
        {
            _set = set;
            _context = context;
        }

        public IQueryable<TData> GetQueryable()
        {
            return _set.AsQueryable();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

    public class SqlStorageProviderFactory : IStorageProviderFactory
    {
        private readonly SqlStorageConfiguration _config;

        public SqlStorageProviderFactory(SqlStorageConfiguration config)
        {
            _config = config;
        }

        public IStorageProvider<TData> Create<TData>() where TData : class
        {
            DbContextOptionsBuilder builder = new DbContextOptionsBuilder();

            ModelBuilder modelBuilder = new ModelBuilder();

            modelBuilder.Entity<TData>().

            DbContextOptions opts = builder.Options;

            DbContext dbContext = new DbContext(opts);

            DbSet<TData> set = dbContext.Set<TData>();

            return new SqlStorageProvider<TData>(set, dbContext);
        }
    }

    public class SqlStorageConfiguration
    {
        public string ConnectionString { get; set; }
        public IDictionary<string, Type> TableMaps { get; set; }
    }
}
