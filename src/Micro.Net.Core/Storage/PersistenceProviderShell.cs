using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Micro.Net.Abstractions.Sagas;
using Micro.Net.Abstractions.Storage;

namespace Micro.Net.Core.Storage
{
    //This is a shim to deal with the MS DI lack of ability to factory open generics
    public class PersistenceProviderShell<TData> : ISagaPersistenceProvider<TData> where TData :  class, ISagaData
    {
        private readonly ISagaPersistenceProvider<TData> _provider;

        public PersistenceProviderShell(ISagaPersistenceProviderFactory factory)
        {
            _provider = factory.Create<TData>();
        }

        public async Task Complete(SagaKey key)
        {
            await _provider.Complete(key);
        }

        public async Task<TData> Get(SagaKey key)
        {
            return await _provider.Get(key);
        }

        public async Task Save(TData obj)
        {
            await _provider.Save(obj);
        }
    }
}
