using System.Threading.Tasks;
using Micro.Net.Handling;

namespace Micro.Net.Abstractions
{
    public interface IHandle<TMessage> where TMessage : IContract
    {
        Task Handle(TMessage message, IHandlerContext context);
    }
    public interface IHandle<TRequest, TResponse> where TRequest : IContract<TResponse>
    {
        Task<TResponse> Handle(TRequest request, IHandlerContext context);
    }
}