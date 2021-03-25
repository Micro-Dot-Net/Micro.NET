using System;

namespace Micro.Net.Host.Abstractions.Sagas
{
    public class SagaFaultContext
    {
        public Exception Ex { get; protected set; }
    }
    
}