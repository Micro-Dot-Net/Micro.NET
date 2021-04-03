using System;

namespace Micro.Net.Abstractions.Sagas
{
    public interface ISagaContract : IContract
    {
        Guid CorrelationId { get; set; }
    }
}