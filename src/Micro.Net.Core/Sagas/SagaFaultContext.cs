using System;

namespace Micro.Net.Abstractions.Sagas
{
    public class SagaFaultContext
    {
        public Exception Ex { get; protected set; }
    }
    
}