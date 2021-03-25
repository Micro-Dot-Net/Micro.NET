using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Micro.Net.Host.Abstractions.Sagas
{
    public interface ISagaContext : IFaultable, IResolvable, ITerminable
    {
        Task DispatchTimeout<TTimeout>() where TTimeout : ISagaTimeout;
    }

    public class SagaContext : ContextBase, ISagaContext
    {
        public async Task DispatchTimeout<TTimeout>() where TTimeout : ISagaTimeout
        {
            throw new NotImplementedException();
        }
        
    }
}