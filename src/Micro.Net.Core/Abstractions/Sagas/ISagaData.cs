using System;

namespace Micro.Net.Abstractions.Sagas
{
    public interface ISagaData
    {
        SagaKey Key { get; }
        Guid CorrelationId { get; }
    }
}