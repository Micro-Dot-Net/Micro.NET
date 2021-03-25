using MediatR;

namespace Micro.Net.Host.Dispatch
{
    public interface IDispatchManager<TRequest, TResponse> : IRequestHandler<DispatchContext<TRequest, TResponse>> where TRequest : IContract<TResponse>
    {
        
    }
}