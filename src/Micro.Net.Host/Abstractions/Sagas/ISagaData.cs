using System;

namespace Micro.Net.Host.Abstractions.Sagas
{
    public interface ISagaData
    {
        Guid CorrelationId { get; }
    }
}