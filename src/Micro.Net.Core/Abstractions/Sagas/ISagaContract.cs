using System;

namespace Micro.Net.Abstractions.Sagas
{
    public interface ISagaContract : IContract
    {
        SagaKey Key { get; set; }
    }
}