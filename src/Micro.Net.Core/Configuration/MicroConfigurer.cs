using System;
using System.Collections.Generic;
using System.Linq;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Storage;
using Micro.Net.Core.Abstractions.Management;
using Micro.Net.Core.Abstractions.Pipeline;
using Micro.Net.Core.Context;
using Micro.Net.Core.Extensions;
using Micro.Net.Core.Hosting;
using Micro.Net.Core.Pipeline;
using Micro.Net.Core.Receive;
using Micro.Net.Core.Storage;
using Micro.Net.Dispatch;
using Micro.Net.Handling;
using Micro.Net.Receive;
using Micro.Net.Serializing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;

namespace Micro.Net.Core.Configuration
{
    public class MicroConfigurer : IMicroConfigurer
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

        public IMicroConfigurer AddHandler<THandler, TRequest, TResponse>() where THandler : IHandle<TRequest,TResponse> where TRequest : IContract<TResponse>
        {
            _components.Add((typeof(THandler),typeof(IHandle<TRequest,TResponse>)), ServiceLifetime.Transient);

            return this;
        }
        public IMicroConfigurer AddHandler<THandler, TMessage>() where THandler : IHandle<TMessage> where TMessage : IContract
        {
            _components.Add((typeof(THandler), typeof(IHandle<TMessage>)), ServiceLifetime.Transient);

            return this;
        }

        public IMicroConfigurer AddDispatcher<TDispatcher>() where TDispatcher : IDispatcher
        {
            _dispatchers.Add(typeof(TDispatcher));

            return this;
        }

        public IMicroConfigurer AddReceiver<TReceiver>() where TReceiver : IReceiver
        {
            _receivers.Add(typeof(TReceiver));

            return this;
        }

        public IMicroConfigurer AddComponent(Action<IServiceCollection> serviceAction)
        {
            _serviceActions.Add(serviceAction);

            return this;
        }

        public IMicroConfigurer AddComponent<TType>(ServiceLifetime lifetime)
        {
            _components.Add((typeof(TType), null), lifetime);

            return this;
        }

        public IMicroConfigurer AddComponent<TInterface, TType>(ServiceLifetime lifetime) where TType : TInterface
        {
            _components.Add((typeof(TType), typeof(TInterface)), lifetime);

            return this;
        }

        public IMicroConfigurer AddReceivable<TRequest, TResponse>()
        {
            _rcvTypes.Add((typeof(TRequest), typeof(TResponse)));

            return this;
        }

        public IMicroConfigurer AddDispatchable<TRequest, TResponse>()
        {
            _dsptTypes.Add((typeof(TRequest), typeof(TResponse)));

            return this;
        }

        public IMicroConfigurer AddReceivable(Type request, Type response)
        {
            _rcvTypes.Add((request, response));

            return this;
        }

        public IMicroConfigurer AddDispatchable(Type request, Type response)
        {
            _dsptTypes.Add((request, response));

            return this;
        }

        public IMicroConfigurer AddSerializer<TSerializer>() where TSerializer : class, ISerializer
        {
            _serviceActions.Add(sc => sc.AddTransient<ISerializer, TSerializer>());

            return this;
        }

        public IMicroConfigurer ConfigureSystem(Action<IMicroSystemConfiguration> cfgAction)
        {
            cfgAction?.Invoke(_config);

            return this;
        }

        internal void populate(IServiceCollection services)
        {
            services.AddHostedService<MicroHostService>();
            services.AddTransient<IReceivePipeFactory, ReceivePipeFactory>();
            services.AddTransient(typeof(IDispatchManager<,>), typeof(DispatchManager<,>));

            services.AddSingleton<IPipeChannel, PipeChannel>();

            foreach ((Type, Type) receivable in _rcvTypes)
            {
                //Type contextType = typeof(ReceiveContext<,>).MakeGenericType(receivable.Item1, receivable.Item2);
                //Type handlerInterface = typeof().MakeGenericType(contextType, typeof(Unit));
                Type shellType;


                if (receivable.Item2 != typeof(ValueTuple))
                {
                    shellType = typeof(HandlerShell<,>).MakeGenericType(receivable.Item1, receivable.Item2);
                }
                else
                {
                    shellType = typeof(HandlerShell<>).MakeGenericType(receivable.Item1);
                }

                services.AddTransient(typeof(IPipelineTail), shellType);
            }

            foreach ((Type, Type) dispatchable in _dsptTypes)
            {
                //Type contextType = typeof(DispatchManagementContext<,>).MakeGenericType(dispatchable.Item1, dispatchable.Item2);
                //Type handlerInterface = typeof(IRequestHandler<,>).MakeGenericType(contextType, typeof(Unit));
                Type shellType = typeof(DispatchManager<,>).MakeGenericType(dispatchable.Item1, dispatchable.Item2);

                services.AddTransient(typeof(IPipelineTail), shellType);
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

            //Perform an under-the-covers switcheroo to deal with MS DI's unfortunate quirks
            if (services.Any(dsc => dsc.ServiceType == typeof(ISagaPersistenceProviderFactory)) && services.Any(dsc => dsc.ServiceType == typeof(ISagaPersistenceProvider<>) && dsc.ImplementationType != typeof(PersistenceProviderShell<>)))
            {
                services.RemoveAll(typeof(ISagaPersistenceProvider<>));
                services.AddTransient(typeof(ISagaPersistenceProvider<>), typeof(PersistenceProviderShell<>));
            }

            services.AddTransient<ISerializerCollection, SerializerCollection>();

            services.AddSingleton<IContextFactory, ContextFactory>();
            services.AddSingleton<IContextSubFactory, ReceiveContextFactory>();
            services.AddSingleton<IContextSubFactory, DispatchContextFactory>();

            if (!_config.TryValidate(out IEnumerable<Exception> configurationExceptions))
            {
                throw new AggregateException(configurationExceptions);
            }

            services.AddTransient<IPipelineTailFactory, ContextFallbackPipeTailFactory>();

            services.AddTransient<MicroSystemConfiguration>(sp => _config.Copy());
        }
    }
}