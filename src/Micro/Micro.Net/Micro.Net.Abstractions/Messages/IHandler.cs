using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Messages
{
    public interface IHandler<TRequest, TResponse> where TRequest : IContract<TResponse>
    {
        Task<TResponse> Handle(TRequest request, IHandlerContext context);
    }
    public interface IHandler<TMessage> where TMessage : IContract
    {
        Task Handle(TMessage message, IHandlerContext context);
    }
}