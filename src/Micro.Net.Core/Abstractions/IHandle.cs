using System.Threading.Tasks;
using Micro.Net.Handling;

namespace Micro.Net.Abstractions
{
    public interface IHandle<TMessage> : IHandle where TMessage : IContract
    {
        Task Handle(TMessage message, HandlerContext context);
    }
    public interface IHandle<TRequest, TResponse> : IHandle where TRequest : IContract<TResponse>
    {
        Task<TResponse> Handle(TRequest request, HandlerContext context);
    }

    public interface IHandle { }
}