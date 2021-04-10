using System;

namespace Micro.Net.Abstractions.Sagas
{
    public interface ISagaFaultContext
    {
        Exception Ex { get; }
    }
}