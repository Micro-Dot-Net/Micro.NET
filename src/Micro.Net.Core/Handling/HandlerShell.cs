using System;
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
    public class HandlerShell<TMessage> : IPipelineTail<ReceiveContext<TMessage,ValueTuple>,ValueTuple> where TMessage : IContract
    {
        private readonly IServiceProvider _provider;


        public HandlerShell(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<ValueTuple> Handle(ReceiveContext<TMessage, ValueTuple> request)
        {
            IHandle<TMessage> svc = _provider.GetService<IHandle<TMessage>>();

            if (request.Request.Payload is ISagaContract)
            {
                //TODO: Add method-caching
                await (Task)this.GetType().GetMethod(nameof(SagaShell.HandleSaga), BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(typeof(TMessage))
                    .Invoke(null, new object[] { request, _provider, CancellationToken.None });
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

    public class HandlerShell<TRequest, TResponse> : IPipelineTail<ReceiveContext<TRequest, TResponse>, ValueTuple>
        where TRequest : IContract<TResponse>
    {
        private readonly IServiceProvider _provider;


        public HandlerShell(IServiceProvider provider)
        {
            _provider = provider;
        }
        

        public async Task<ValueTuple> Handle(ReceiveContext<TRequest, TResponse> request)
        {
            HandlerContext context = new HandlerContext(_provider.GetService<IPipeChannel>(), _provider.GetService<MicroSystemConfiguration>());

            //TODO: Cache branches/types
            //TODO: Set up terminate handler and resolve handler
            if (typeof(TResponse) != typeof(ValueTuple))
            {
                IHandle<TRequest, TResponse> svc = _provider.GetService<IHandle<TRequest, TResponse>>();

                request.Response.Payload = await svc.Handle(request.Request.Payload, context);
            }
            else
            {
                dynamic svc = _provider.GetService(typeof(IHandle<>).MakeGenericType(typeof(TRequest)));

                await svc.Handle(request.Request.Payload, context);

                request.Response.Payload = default;
            }

            return ValueTuple.Create();
        }
    }
}