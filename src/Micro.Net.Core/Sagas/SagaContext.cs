using System;
using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Sagas
{
    public class SagaContext : ContextBase, ISagaContext
    {
        public async Task DispatchTimeout<TTimeout>() where TTimeout : ISagaTimeout
        {
            throw new NotImplementedException();
        }
        
    }
}