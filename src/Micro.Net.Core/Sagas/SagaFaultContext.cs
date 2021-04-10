using System;

namespace Micro.Net.Abstractions.Sagas
{
    public class SagaFaultContext : ISagaFaultContext
    {
        public Exception Ex { get; protected set; }
    }
    
}