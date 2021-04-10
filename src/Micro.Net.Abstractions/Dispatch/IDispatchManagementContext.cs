using Micro.Net.Abstractions;

namespace Micro.Net.Dispatch
{
    public interface IDispatchManagementContext<TRequest, TResponse> : IDispatchContext<TRequest, TResponse> where TRequest : IContract<TResponse>
    {

    }
}