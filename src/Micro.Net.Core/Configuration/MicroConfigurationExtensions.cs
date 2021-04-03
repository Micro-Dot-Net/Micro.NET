using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using MediatR;
using Micro.Net.Abstractions;
using Micro.Net.Core.Abstractions.Management;
using Micro.Net.Core.Hosting;
using Micro.Net.Core.Receive;
using Micro.Net.Dispatch;
using Micro.Net.Handling;
using Micro.Net.Receive;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Micro.Net.Core.Configuration
{
    public static class MicroConfigurationExtensions
    {
        public static IServiceCollection UseMicroNet(this IServiceCollection collection, Action<MicroConfigurer> configureAction)
        {
            MicroConfigurer cfgr = new MicroConfigurer();

            configureAction(cfgr);

            cfgr.populate(collection);

            return collection;
        }
    }

    public class MicroConfigurer
    {
        internal MicroConfigurer()
        {

        }

        private ISet<(Type, Type)> _rcvTypes = new HashSet<(Type, Type)>();
        private ISet<(Type, Type)> _dsptTypes = new HashSet<(Type, Type)>();
        private ISet<Type> _dispatchers = new HashSet<Type>();
        private ISet<Type> _receivers = new HashSet<Type>();
        private List<Action<IServiceCollection>> _serviceActions = new List<Action<IServiceCollection>>();
        private IDictionary<(Type, Type), ServiceLifetime> _components = new Dictionary<(Type, Type), ServiceLifetime>();

        private MicroSystemConfiguration _config = new MicroSystemConfiguration()
            {Dispatch = new MicroSystemDispatchConfiguration()};

        public MicroConfigurer AddHandler<THandler, TRequest, TResponse>() where THandler : IHandle<TRequest,TResponse> where TRequest : IContract<TResponse>
        {
            _components.Add((typeof(THandler),typeof(IHandle<TRequest,TResponse>)), ServiceLifetime.Transient);

            return this;
        }
        public MicroConfigurer AddHandler<THandler, TMessage>() where THandler : IHandle<TMessage> where TMessage : IContract
        {
            _components.Add((typeof(THandler), typeof(IHandle<TMessage>)), ServiceLifetime.Transient);

            return this;
        }

        public MicroConfigurer AddDispatcher<TDispatcher>() where TDispatcher : IDispatcher
        {
            _dispatchers.Add(typeof(TDispatcher));

            return this;
        }

        public MicroConfigurer AddReceiver<TReceiver>() where TReceiver : IReceiver
        {
            _receivers.Add(typeof(TReceiver));

            return this;
        }

        public MicroConfigurer AddComponent(Action<IServiceCollection> serviceAction)
        {
            _serviceActions.Add(serviceAction);

            return this;
        }

        public MicroConfigurer AddComponent<TType>(ServiceLifetime lifetime)
        {
            _components.Add((typeof(TType), null), lifetime);

            return this;
        }

        public MicroConfigurer AddComponent<TInterface, TType>(ServiceLifetime lifetime) where TType : TInterface
        {
            _components.Add((typeof(TType), typeof(TInterface)), lifetime);

            return this;
        }

        public MicroConfigurer AddReceivable<TRequest, TResponse>()
        {
            _rcvTypes.Add((typeof(TRequest), typeof(TResponse)));

            return this;
        }

        public MicroConfigurer AddDispatchable<TRequest, TResponse>()
        {
            _dsptTypes.Add((typeof(TRequest), typeof(TResponse)));

            return this;
        }

        public MicroConfigurer AddReceivable(Type request, Type response)
        {
            _rcvTypes.Add((request, response));

            return this;
        }

        public MicroConfigurer AddDispatchable(Type request, Type response)
        {
            _dsptTypes.Add((request, response));

            return this;
        }

        public MicroConfigurer Configure(Action<MicroSystemConfiguration> cfgAction)
        {
            cfgAction?.Invoke(_config);

            return this;
        }

        internal void populate(IServiceCollection services)
        {
            services.AddHostedService<MicroHostService>();
            services.AddTransient<IReceivePipeFactory, ReceivePipeFactory>();
            services.AddTransient(typeof(IDispatchManager<,>), typeof(DispatchManager<,>));

            services.AddMediatR(cfg =>
            {
                cfg.AsSingleton();
            }, typeof(MicroConfigurer));

            foreach ((Type, Type) receivable in _rcvTypes)
            {
                Type contextType = typeof(ReceiveContext<,>).MakeGenericType(receivable.Item1, receivable.Item2);
                Type handlerInterface = typeof(IRequestHandler<,>).MakeGenericType(contextType, typeof(Unit));
                Type shellType;


                if (receivable.Item2 != typeof(ValueTuple))
                {
                    shellType = typeof(HandlerShell<,>).MakeGenericType(receivable.Item1, receivable.Item2);
                }
                else
                {
                    shellType = typeof(HandlerShell<>).MakeGenericType(receivable.Item1);
                }

                services.AddTransient(handlerInterface, shellType);
            }

            foreach ((Type, Type) dispatchable in _dsptTypes)
            {
                Type contextType = typeof(DispatchManagementContext<,>).MakeGenericType(dispatchable.Item1, dispatchable.Item2);
                Type handlerInterface = typeof(IRequestHandler<,>).MakeGenericType(contextType, typeof(Unit));
                Type shellType = typeof(DispatchManager<,>).MakeGenericType(dispatchable.Item1, dispatchable.Item2);

                services.AddTransient(handlerInterface, shellType);
            }

            foreach (Type type in _dispatchers)
            {
                services.AddTransient(typeof(IDispatcher), type);
                services.AddTransient(typeof(IMicroComponent), type);
            }

            foreach (Type receiver in _receivers)
            {
                services.AddTransient(typeof(IReceiver), receiver);
                services.AddTransient(typeof(IMicroComponent), receiver);
            }

            foreach (Action<IServiceCollection> serviceAction in _serviceActions)
            {
                serviceAction(services);
            }

            foreach (KeyValuePair<(Type, Type),ServiceLifetime> component in _components)
            {
                if (component.Key.Item2 != null)
                {
                    services.TryAdd(ServiceDescriptor.Describe(component.Key.Item2, component.Key.Item1, component.Value));
                }
                else
                {
                    services.TryAdd(ServiceDescriptor.Describe(component.Key.Item1, component.Key.Item1, component.Value));
                }
            }

            services.AddSingleton(_config);
        }
    }
}
