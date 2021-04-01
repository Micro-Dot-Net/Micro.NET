using MediatR;
using Micro.Net.Abstractions;

namespace Micro.Net.Dispatch
{
    public interface IDispatchManager<TRequest, TResponse> : IRequestHandler<DispatchManagementContext<TRequest, TResponse>> where TRequest : IContract<TResponse>
    {
        
    }
}