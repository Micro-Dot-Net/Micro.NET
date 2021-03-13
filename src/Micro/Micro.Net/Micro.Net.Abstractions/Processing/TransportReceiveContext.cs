using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GreenPipes;

namespace Micro.Net.Abstractions.Processing
{
    public class TransportReceiveContext : PipeContext
    {
        public bool HasPayloadType(Type payloadType)
        {
            throw new NotImplementedException();
        }

        public bool TryGetPayload<T>(out T payload) where T : class
        {
            throw new NotImplementedException();
        }

        public T GetOrAddPayload<T>(PayloadFactory<T> payloadFactory) where T : class
        {
            throw new NotImplementedException();
        }

        public T AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory) where T : class
        {
            throw new NotImplementedException();
        }

        public CancellationToken CancellationToken { get; }
    }
}
