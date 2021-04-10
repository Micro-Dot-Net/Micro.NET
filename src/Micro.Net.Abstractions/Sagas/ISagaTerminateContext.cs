using System.Collections.Generic;

namespace Micro.Net.Abstractions.Sagas
{
    public interface ISagaTerminateContext<TData>
    {
        TData Data { get; }
        string Reason { get; }
        IReadOnlyDictionary<string, object> AuxiliaryData { get; }
    }
}

namespace Micro.Net.Abstractions.Sagas
{
    public interface ISagaTerminateContext
    {
        string Reason { get; }
        IReadOnlyDictionary<string, object> AuxiliaryData { get; }
    }
}