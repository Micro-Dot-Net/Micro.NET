using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Sagas;
using Micro.Net.Core.Abstractions.Pipeline;
using Micro.Net.Core.Pipeline;
using Micro.Net.Receive;
using Micro.Net.Sagas;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Net.Handling
{
    internal static class SagaCache
    {
        public static readonly IDictionary<Type, MethodInfo> _sagaHandleCache = new ConcurrentDictionary<Type, MethodInfo>();

        public static readonly IDictionary<Type, Type> _sagaDataMessageMapCache = new ConcurrentDictionary<Type, Type>();

        public static readonly IDictionary<Type, MethodInfo> _slSagaHandleCache = new ConcurrentDictionary<Type, MethodInfo>();

        public static readonly IDictionary<(Type, Type), Func<object, SagaFinderContext, object>> _slSagaDataFindCache = new ConcurrentDictionary<(Type, Type), Func<object, SagaFinderContext, object>>();

        public static readonly IDictionary<Type, Func<object>> _slSagaStartCache = new ConcurrentDictionary<Type, Func<object>>();
    }

    internal static class HandlerCache
    {
        public static readonly IDictionary<(Type, Type), Func<object, HandlerContext, Task<object>>> _handleCache =
            new ConcurrentDictionary<(Type, Type), Func<object, HandlerContext, Task<object>>>();
    }

    public class HandlerShell<TMessage> : IPipelineTail<IReceiveContext<TMessage,ValueTuple>,ValueTuple> where TMessage : IContract
    {
        private readonly IServiceProvider _provider;

        public HandlerShell(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<ValueTuple> Handle(IReceiveContext<TMessage, ValueTuple> request)
        {
            IHandle<TMessage> svc = _provider.GetService<IHandle<TMessage>>();

            if (request.Request.Payload is ISagaContract)
            {
                SagaCache._sagaHandleCache[request.Request.Payload.GetType()] ??= this.GetType().GetMethod(nameof(SagaShell.HandleSaga), BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(typeof(TMessage));
                
                await (Task)SagaCache._sagaHandleCache[request.Request.Payload.GetType()].Invoke(null, new object[] { request, _provider, CancellationToken.None });
            }
            else
            {
                HandlerContext context = new HandlerContext(_provider.GetService<IPipeChannel>(), _provider.GetService<MicroSystemConfiguration>());

                await svc.Handle(request.Request.Payload, context);

                request.Response.Payload = new ValueTuple();
                //TODO: Set up terminate handler and resolve handler
            }

            return ValueTuple.Create();
        }
    }

    public class HandlerShell<TRequest, TResponse> : IPipelineTail<IReceiveContext<TRequest, TResponse>, ValueTuple>
        where TRequest : IContract<TResponse>
    {
        private readonly IServiceProvider _provider;


        public HandlerShell(IServiceProvider provider)
        {
            _provider = provider;
        }
        

        public async Task<ValueTuple> Handle(IReceiveContext<TRequest, TResponse> request)
        {
            HandlerContext context = new HandlerContext(_provider.GetService<IPipeChannel>(), _provider.GetService<MicroSystemConfiguration>());

            Func<object, HandlerContext, Task<object>> _handle = null;

            if (HandlerCache._handleCache.TryGetValue((typeof(TRequest), typeof(TResponse)), out Func<object, HandlerContext, Task<object>> handle))
            {
                _handle = handle;
            }
            
            if (_handle == null)
            {
                if (typeof(TResponse) != typeof(ValueTuple))
                {
                    IHandle<TRequest, TResponse> svc = _provider.GetService<IHandle<TRequest, TResponse>>();

                    HandlerCache._handleCache[(typeof(TRequest), typeof(TResponse))] = _handle = async (obj, ctx) =>
                    {
                        return await svc.Handle(request.Request.Payload, context);
                    };
                }
                else
                {
                    dynamic svc = _provider.GetService(typeof(IHandle<>).MakeGenericType(typeof(TRequest)));

                    HandlerCache._handleCache[(typeof(TRequest), typeof(TResponse))] = _handle = async (obj, ctx) =>
                    {
                        await svc.Handle(request.Request.Payload, context);

                        return default(TResponse);
                    };
                }
            }

            request.Response.Payload = (TResponse) await _handle.Invoke(request, context);

            return ValueTuple.Create();
        }
    }
}