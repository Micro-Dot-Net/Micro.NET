using System;
using System.Threading.Tasks;
using MediatR;
using Micro.Net.Abstractions;
using Micro.Net.Dispatch;
using Micro.Net.Exceptions;

namespace Micro.Net.Handling
{
    public class HandlerContext : IHandlerContext
    {
        private readonly IMediator _mediator;
        private readonly MicroSystemConfiguration _config;

        public HandlerContext(IMediator mediator, MicroSystemConfiguration config)
        {
            _mediator = mediator;
            _config = config;
        }

        public async Task<TResponse> Dispatch<TRequest, TResponse>(TRequest request, Action<DispatchOptions> ctxAction = null, bool? throwOnFault = null) where TRequest : IContract<TResponse>
        {
            DispatchManagementContext<TRequest, TResponse> context = DispatchManagementContext<TRequest, TResponse>.Create(request);

            ctxAction?.Invoke(context.Options);

            await _mediator.Send(context);

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

            await _mediator.Send(context);

            if (context.TryGetFault(out Exception ex) && (throwOnFault ?? _config.Dispatch.ThrowOnFault ?? false))
            {
                throw ex;
            }
        }
    }
}