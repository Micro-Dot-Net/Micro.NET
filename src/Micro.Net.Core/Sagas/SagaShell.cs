using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Sagas;
using Micro.Net.Exceptions;
using Micro.Net.Receive;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Net.Sagas
{
    public static class SagaShell
    {
        //TODO: Cache branching
        internal static async Task HandleSaga<TSagaMessage>(ReceiveContext<TSagaMessage, ValueTuple> request, IServiceProvider provider, CancellationToken cancellationToken) where TSagaMessage : ISagaContract
        {
            ISagaStepHandler<TSagaMessage> _svc = provider.GetService<ISagaStepHandler<TSagaMessage>>();

            if (_svc == null)
            {
                //TODO: Better exception here
                request.SetFault(new MicroHostException());

                return;
            }

            //Seek saga data type
            Type svcType = _svc.GetType();

            while ((svcType.IsGenericType ? svcType.GetGenericTypeDefinition() : svcType) != typeof(Saga<>))
            {
                svcType = svcType.BaseType;
            }

            //TODO: Cache Message -> SagaData map
            Type sagaDataType = svcType.GetGenericArguments()[0];

            //TODO: Cache method construction
            await (Task)MethodBase.GetCurrentMethod().DeclaringType.GetMethod(nameof(HandleSagaData), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(typeof(TSagaMessage),sagaDataType)
                .Invoke(null, new object[] { request, provider, _svc, cancellationToken });
        }

        internal static async Task HandleSagaData<TSagaMessage, TSagaData>(ReceiveContext<TSagaMessage, ValueTuple> request, IServiceProvider provider, ISagaStepHandler<TSagaMessage> stepHandler, CancellationToken cancellationToken) where TSagaMessage : ISagaContract where TSagaData : class, ISagaData
        {
            SagaFinderContext finderContext = new SagaFinderContext();

            TSagaData data = default;

            //TODO: Cache (Message,Data) -> Finder, check if can be done preemptively?
            {
                ISagaFinder<TSagaData>.IFor<TSagaMessage> finder =
                    provider.GetService<ISagaFinder<TSagaData>.IFor<TSagaMessage>>();

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

            //TODO: Cache branching
            if (data == null && !(stepHandler is ISagaStartHandler<TSagaMessage>))
            {
                //TODO: Set better exception
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
                        //TODO: Set better exception
                        request.SetFault(new MicroException());

                        return;
                    }

                    data = factory.Create();
                }
            }

            ISagaContext sagaContext = new SagaContext();

            //TODO: Cache branching
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
}