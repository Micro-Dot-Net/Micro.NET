using Micro.Net.Abstractions.Messages;

namespace Micro.Net.Abstractions.Configuration
{
    public interface IHandlerConfigurable
    {
        IHandlerConfigurable UseHandler<THandler, TMessage>() where THandler : IHandler<TMessage> where TMessage : IContract;

        IHandlerConfigurable UseHandler<THandler, TRequest, TResponse>() where THandler : IHandler<TRequest, TResponse> where TRequest : IContract<TResponse>;
    }
}