using Micro.Net.Abstractions.Storage;
using Microsoft.EntityFrameworkCore;

namespace Micro.Net.Storage.Sql
{
    public class SqlStorageProviderFactory : IPersistenceProviderFactory
    {
        private readonly SqlStorageConfiguration _config;

        public SqlStorageProviderFactory(SqlStorageConfiguration config)
        {
            _config = config;
        }

        public IPersistenceProvider<TData> Create<TData>() where TData : class
        {
            DbContextOptionsBuilder builder = new DbContextOptionsBuilder();

            ModelBuilder modelBuilder = new ModelBuilder();

            //modelBuilder.Entity<TData>().

            DbContextOptions opts = builder.Options;

            DbContext dbContext = new DbContext(opts);

            DbSet<TData> set = dbContext.Set<TData>();

            return new SqlStorageProvider<TData>(set, dbContext);
        }
    }
}