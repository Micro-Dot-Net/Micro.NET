using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Sagas;
using Micro.Net.Exceptions;
using Micro.Net.Handling;
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

            Type sagaDataType;
                
            if(SagaCache._sagaDataMessageMapCache.TryGetValue(typeof(TSagaMessage), out Type _sagaDataType))
            {
                sagaDataType = _sagaDataType;
            }
            else
            {
                //Seek saga data type
                Type svcType = _svc.GetType();

                while ((svcType.IsGenericType ? svcType.GetGenericTypeDefinition() : svcType) != typeof(Saga<>))
                {
                    svcType = svcType.BaseType;
                }
                
                sagaDataType = SagaCache._sagaDataMessageMapCache[typeof(TSagaMessage)] = svcType.GetGenericArguments()[0];
            }

            MethodInfo slSagaHandleMethod;

            if (SagaCache._slSagaHandleCache.TryGetValue(typeof(TSagaMessage), out MethodInfo _slSagaHandleMethod))
            {
                slSagaHandleMethod = _slSagaHandleMethod;
            }
            else
            {
                slSagaHandleMethod = SagaCache._slSagaHandleCache[typeof(TSagaMessage)] = MethodBase.GetCurrentMethod()
                    .DeclaringType.GetMethod(nameof(HandleSagaData), BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(typeof(TSagaMessage), sagaDataType);
            }
            
            await (Task)slSagaHandleMethod.Invoke(null, new object[] { request, provider, _svc, cancellationToken });
        }

        internal static async Task HandleSagaData<TSagaMessage, TSagaData>(ReceiveContext<TSagaMessage, ValueTuple> request, IServiceProvider provider, ISagaStepHandler<TSagaMessage> stepHandler, CancellationToken cancellationToken) where TSagaMessage : ISagaContract where TSagaData : class, ISagaData
        {
            SagaFinderContext finderContext = new SagaFinderContext();
            
            TSagaData data = default;

            Func<object, SagaFinderContext, object> finderFunc = null;

            if (SagaCache._slSagaDataFindCache.TryGetValue((typeof(TSagaMessage), typeof(TSagaData)), out Func<object, SagaFinderContext, object> _finderFunc))
            {
                finderFunc = _finderFunc;
            }
            else
            {
                {
                    ISagaFinder<TSagaData>.IFor<TSagaMessage> finder =
                        provider.GetService<ISagaFinder<TSagaData>.IFor<TSagaMessage>>();

                    if (finder != null)
                    {
                        data = await finder.Find(request.Request.Payload, finderContext);
                    }
                }

                if (data == default)
                {
                    IDefaultSagaFinder<TSagaData> finder = provider.GetService<IDefaultSagaFinder<TSagaData>>();

                    if (finder != null)
                    {
                        data = await finder.Find(request.Request.Payload, finderContext);
                    }
                }

                if (data == default)
                {
                    IDefaultSagaFinder finder = provider.GetService<IDefaultSagaFinder>();

                    if (finder != null)
                    {
                        data = await finder.Find<TSagaData, TSagaMessage>(request.Request.Payload, finderContext);
                    }
                }
            }

            if (finderFunc == null)
            {
                //TODO: Set better exception
                request.SetFault(new MicroException());

                return;
            }

            data = (TSagaData) finderFunc.Invoke(request.Request.Payload, finderContext);
            
            if (data == default && !(stepHandler is ISagaStartHandler<TSagaMessage>))
            {
                //TODO: Set better exception
                request.SetFault(new MicroException());

                return;
            }
            
            if (data == default)
            {
                Func<object> slSagaStart = null;

                if (SagaCache._slSagaStartCache.TryGetValue(typeof(TSagaData), out Func<object> _slSagaStart))
                {
                    slSagaStart = _slSagaStart;
                }
                else
                {
                    if (typeof(TSagaData).GetConstructor(Type.EmptyTypes) == null)
                    {
                        slSagaStart = SagaCache._slSagaStartCache[typeof(TSagaData)] = () => Activator.CreateInstance<TSagaData>();
                    }
                    else
                    {
                        IFactory<TSagaData> factory = provider.GetService<IFactory<TSagaData>>();

                        if (factory != null)
                        {
                            slSagaStart = SagaCache._slSagaStartCache[typeof(TSagaData)] = () => factory.Create();
                        }
                    }
                }

                if (slSagaStart == null)
                {
                    //TODO: Set better exception
                    request.SetFault(new MicroException());

                    return;
                }

                data = (TSagaData)slSagaStart.Invoke();
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
                //TODO: Set better exception
                request.SetFault(new MicroException());

                return;
            }
        }
    }
}