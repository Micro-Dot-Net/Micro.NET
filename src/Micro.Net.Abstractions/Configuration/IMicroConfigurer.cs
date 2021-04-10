using System;
using Micro.Net.Abstractions;
using Micro.Net.Core.Receive;
using Micro.Net.Dispatch;
using Micro.Net.Handling;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Net.Core.Configuration
{
    public interface IMicroConfigurer
    {
        IMicroConfigurer AddHandler<THandler, TRequest, TResponse>() where THandler : IHandle<TRequest,TResponse> where TRequest : IContract<TResponse>;
        IMicroConfigurer AddHandler<THandler, TMessage>() where THandler : IHandle<TMessage> where TMessage : IContract;
        IMicroConfigurer AddDispatcher<TDispatcher>() where TDispatcher : IDispatcher;
        IMicroConfigurer AddReceiver<TReceiver>() where TReceiver : IReceiver;
        IMicroConfigurer AddComponent(Action<IServiceCollection> serviceAction);
        IMicroConfigurer AddComponent<TType>(ServiceLifetime lifetime);
        IMicroConfigurer AddComponent<TInterface, TType>(ServiceLifetime lifetime) where TType : TInterface;
        IMicroConfigurer AddReceivable<TRequest, TResponse>();
        IMicroConfigurer AddDispatchable<TRequest, TResponse>();
        IMicroConfigurer AddReceivable(Type request, Type response);
        IMicroConfigurer AddDispatchable(Type request, Type response);
        IMicroConfigurer AddSerializer<TSerializer>() where TSerializer : class, ISerializer;
        IMicroConfigurer ConfigureSystem(Action<IMicroSystemConfiguration> cfgAction);
    }
}