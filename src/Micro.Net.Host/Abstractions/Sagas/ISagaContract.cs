using System;

namespace Micro.Net.Abstractions.Sagas
{
    public interface ISagaContract : IContract
    {
        public Guid CorrelationId { get; set; }
    }
}