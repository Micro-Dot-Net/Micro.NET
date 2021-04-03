using MediatR;
using Micro.Net.Abstractions;
using Micro.Net.Core.Abstractions.Management;

namespace Micro.Net.Dispatch
{
    public interface IDispatchManager<TRequest, TResponse> : IRequestHandler<DispatchManagementContext<TRequest, TResponse>>, IMicroComponent where TRequest : IContract<TResponse>
    {
        
    }
}