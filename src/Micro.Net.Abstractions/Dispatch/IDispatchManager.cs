using System;
using Micro.Net.Abstractions;
using Micro.Net.Core.Abstractions.Management;
using Micro.Net.Core.Pipeline;

namespace Micro.Net.Dispatch
{
    public interface IDispatchManager<TRequest, TResponse> : IPipelineTail<IDispatchManagementContext<TRequest, TResponse>, ValueTuple>, IMicroComponent where TRequest : IContract<TResponse>
    {
        
    }
}