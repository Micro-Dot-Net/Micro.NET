using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Sagas;
using Micro.Net.Receive;
using Micro.Net.Sagas;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Net.Handling
{
    public class HandlerShell<TMessage> : IRequestHandler<ReceiveContext<TMessage, ValueTuple>> where TMessage : IContract
    {
        private readonly IServiceProvider _provider;


        public HandlerShell(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<Unit> Handle(ReceiveContext<TMessage, ValueTuple> request, CancellationToken cancellationToken)
        {
            IHandle<TMessage> svc = _provider.GetService<IHandle<TMessage>>();

            if (request.Request.Payload is ISagaContract)
            {
                //TODO: Add method-caching
                await (Task)this.GetType().GetMethod(nameof(SagaShell.HandleSaga), BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(typeof(TMessage))
                    .Invoke(null, new object[] {request, _provider, cancellationToken});
            }
            else
            {
                HandlerContext context = new HandlerContext(_provider.GetService<IMediator>(), _provider.GetService<MicroSystemConfiguration>());

                await svc.Handle(request.Request.Payload, context);

                request.Response.Payload = new ValueTuple();
                //TODO: Set up terminate handler and resolve handler
            }

            return Unit.Value;
        }
    }

    public class HandlerShell<TRequest, TResponse> : IRequestHandler<ReceiveContext<TRequest, TResponse>>
        where TRequest : IContract<TResponse>
    {
        private readonly IServiceProvider _provider;


        public HandlerShell(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<Unit> Handle(ReceiveContext<TRequest, TResponse> notification, CancellationToken cancellationToken)
        {
            HandlerContext context = new HandlerContext(_provider.GetService<IMediator>(), _provider.GetService<MicroSystemConfiguration>());

            //TODO: Cache branches/types
            //TODO: Set up terminate handler and resolve handler
            if (typeof(TResponse) != typeof(ValueTuple))
            {
                IHandle<TRequest, TResponse> svc = _provider.GetService<IHandle<TRequest, TResponse>>();

                notification.Response.Payload = await svc.Handle(notification.Request.Payload, context);
            }
            else
            {
                dynamic svc = _provider.GetService(typeof(IHandle<>).MakeGenericType(typeof(TRequest)));

                await svc.Handle(notification.Request.Payload, context);

                notification.Response.Payload = default;
            }

            return Unit.Value;
        }
    }
}