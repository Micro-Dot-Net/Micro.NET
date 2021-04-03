using System.Collections.Generic;

namespace Micro.Net.Abstractions.Sagas
{
    public abstract class SagaTerminateContext
    {
        public string Reason { get; protected set; }
        public IReadOnlyDictionary<string, object> AuxiliaryData { get; protected set; }
    }

    public abstract class SagaTerminateContext<TData> : SagaTerminateContext
    {
        public TData Data { get; protected set; }
    }
}