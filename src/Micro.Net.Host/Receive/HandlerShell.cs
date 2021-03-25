using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Micro.Net.Host.Abstractions;
using Micro.Net.Host.Abstractions.Sagas;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Net
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
                await (Task)this.GetType().GetMethod(nameof(HandleSaga), BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(typeof(TMessage))
                    .Invoke(null, new object[] {request, _provider, cancellationToken});
            }
            else
            {
                HandlerContext context = new HandlerContext(_provider.GetService<IMediator>());

                await svc.Handle(request.Request.Payload, context);

                request.Response.Payload = new ValueTuple();
            }

            return Unit.Value;
        }

        private static async Task HandleSaga<TSagaMessage>(ReceiveContext<TSagaMessage, ValueTuple> request, IServiceProvider provider, CancellationToken cancellationToken) where TSagaMessage : ISagaContract
        {
            SagaContext context = new SagaContext();

            Type startHandlerType = typeof(ISagaStartHandler<TSagaMessage>).MakeGenericType(typeof(TSagaMessage));
            Type stepHandlerType = typeof(ISagaStepHandler<>).MakeGenericType(typeof(TSagaMessage));

            ISagaStepHandler<TSagaMessage> _svc = provider.GetService<ISagaStepHandler<TSagaMessage>>();

            if (_svc == null)
            {
                request.SetFault(new MicroHostException());

                return;
            }

            //Seek saga data type
            Type svcType = _svc.GetType();

            while ((svcType.IsGenericType ? svcType.GetGenericTypeDefinition() : svcType) != typeof(Saga<>))
            {
                svcType = svcType.BaseType;
            }

            Type sagaDataType = svcType.GetGenericArguments()[0];

            await (Task)MethodBase.GetCurrentMethod().DeclaringType.GetMethod(nameof(HandleSagaData), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(typeof(TMessage), sagaDataType)
                .Invoke(null, new object[] { request, provider, _svc, cancellationToken });
        }

        private static async Task HandleSagaData<TSagaMessage, TSagaData>(ReceiveContext<TSagaMessage, ValueTuple> request, IServiceProvider provider, ISagaStepHandler<TSagaMessage> stepHandler, CancellationToken cancellationToken) where TSagaMessage : ISagaContract where TSagaData : class, ISagaData
        {
            SagaFinderContext finderContext = new SagaFinderContext();

            TSagaData data = default;

            {
                ISagaFinder<TSagaData>.For<TSagaMessage> finder =
                    provider.GetService<ISagaFinder<TSagaData>.For<TSagaMessage>>();

                if (finder != null)
                {
                    data = await finder.Find(request.Request.Payload, finderContext);
                }
            }

            if (data == null)
            {
                IDefaultSagaFinder<TSagaData> finder = provider.GetService<IDefaultSagaFinder<TSagaData>>();

                if (finder != null)
                {
                    data = await finder.Find(request.Request.Payload, finderContext);
                }
            }

            if (data == null)
            {
                IDefaultSagaFinder finder = provider.GetService<IDefaultSagaFinder>();

                if (finder != null)
                {
                    data = await finder.Find<TSagaData,TSagaMessage>(request.Request.Payload, finderContext);
                }
            }

            if (data == null && !(stepHandler is ISagaStartHandler<TSagaMessage>))
            {
                request.SetFault(new MicroException());

                return;
            }
            else if(data == null)
            {
                if (typeof(TSagaData).GetConstructor(Type.EmptyTypes) != null)
                {
                    data = Activator.CreateInstance<TSagaData>();
                }
                else
                {
                    IFactory<TSagaData> factory = provider.GetService<IFactory<TSagaData>>();

                    if (factory == null)
                    {
                        request.SetFault(new MicroException());

                        return;
                    }

                    data = factory.Create();
                }
            }

            ISagaContext sagaContext = new SagaContext();

            if (stepHandler is Saga<TSagaData> saga)
            {
                saga.Data = data;

                await stepHandler.Handle(request.Request.Payload, sagaContext);

                if (sagaContext.TryGetFault(out Exception ex))
                {
                    ISagaFaultHandler faultHandler = provider.GetService<ISagaFaultHandler>();

                    if (faultHandler != null)
                    {
                        SagaFaultContext context = new SagaFaultContext();

                        await faultHandler.HandleFault(request.Request.Payload, data, context);
                    }
                        
                    request.SetFault(ex);
                }

                //TODO: Set up terminate handler and resolve handler
            }
            else
            {
                request.SetFault(new MicroException());

                return;
            }
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
            HandlerContext context = new HandlerContext(_provider.GetService<IMediator>());



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