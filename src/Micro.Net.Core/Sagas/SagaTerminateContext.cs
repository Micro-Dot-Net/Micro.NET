using System.Collections.Generic;

namespace Micro.Net.Abstractions.Sagas
{
    public class SagaTerminateContext : ISagaTerminateContext
    {
        public string Reason { get; protected set; }
        public IReadOnlyDictionary<string, object> AuxiliaryData { get; protected set; }
    }

    public class SagaTerminateContext<TData> : SagaTerminateContext, ISagaTerminateContext<TData>
    {
        public TData Data { get; protected set; }
    }
}