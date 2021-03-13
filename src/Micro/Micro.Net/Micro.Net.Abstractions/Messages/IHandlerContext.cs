using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Messages
{
    public interface IHandlerContext
    {
        Task<TResponse> Dispatch<TRequest, TResponse>(TRequest request) where TRequest : IContract<TResponse>;
        Task Dispatch<TRequest>(TRequest request) where TRequest : IContract;
    }
}