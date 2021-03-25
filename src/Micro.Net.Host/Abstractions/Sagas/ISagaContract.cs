using System;

namespace Micro.Net.Host.Abstractions.Sagas
{
    public interface ISagaContract : IContract
    {
        public Guid CorrelationId { get; set; }
    }
}