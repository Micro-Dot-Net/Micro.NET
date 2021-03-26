using System;

namespace Micro.Net.Abstractions.Sagas
{
    public interface ISagaData
    {
        Guid CorrelationId { get; }
    }
}