using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Timeout;
using Micro.Net.Core.Abstractions.Pipeline;
using Micro.Net.Core.Timeout;
using Micro.Net.Dispatch;
using Micro.Net.Exceptions;
using Micro.Net.Receive;
using NodaTime;
using OneOf;

namespace Micro.Net.Handling
{
    public class HandlerContext : IHandlerContext
    {
        private readonly IPipeChannel _pipeChannel;
        private readonly MicroSystemConfiguration _config;

        public HandlerContext(IPipeChannel pipeChannel, MicroSystemConfiguration config)
        {
            _pipeChannel = pipeChannel;
            _config = config;
        }

        public async Task<TResponse> Dispatch<TRequest, TResponse>(TRequest request, Action<DispatchOptions> ctxAction = null, bool? throwOnFault = null) where TRequest : IContract<TResponse>
        {
            DispatchManagementContext<TRequest, TResponse> context = DispatchManagementContext<TRequest, TResponse>.Create(request);

            ctxAction?.Invoke(context.Options);

            await _pipeChannel.Handle(context);

            if (context.TryGetFault(out Exception ex) && (throwOnFault ?? _config.Dispatch.ThrowOnFault ?? false))
            {
                throw ex;
            }

            return context.Response.Payload;
        }

        public async Task Dispatch<TRequest>(TRequest request, Action<DispatchOptions> ctxAction = null, bool? throwOnFault = null) where TRequest : IContract
        {
            DispatchManagementContext<TRequest, ValueTuple> context = DispatchManagementContext<TRequest, ValueTuple>.Create(request);

            ctxAction?.Invoke(context.Options);

            await _pipeChannel.Handle(context);

            if (context.TryGetFault(out Exception ex) && (throwOnFault ?? _config.Dispatch.ThrowOnFault ?? false))
            {
                throw ex;
            }
        }
        
        public async Task RequestTimeout<TTimeout>(TTimeout timeout, OneOf<Instant,Duration> delay, Action<TimeoutOptions> ctxAction = null, Action<IDictionary<string, string>> headerAction = null) where TTimeout : ITimeout
        {
            ITimeoutContext<TTimeout> timeoutContext = new TimeoutContext<TTimeout>()
            {
                Delay = delay,
                Options = new TimeoutOptions(),
                Request = new RequestContext<TTimeout>()
                {
                    Headers = new Dictionary<string, string>(),
                    Payload = timeout
                }
            };

            ctxAction?.Invoke(timeoutContext.Options);

            headerAction?.Invoke(timeoutContext.Request.Headers);

            await _pipeChannel.Handle(timeoutContext);

            if (!timeoutContext.IsResolved)
            {
                //TODO: Throw if pipetail wasn't found
            }
        }
    }
}