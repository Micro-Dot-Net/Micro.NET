using System;
using System.Threading.Tasks;
using MediatR;
using Micro.Net.Abstractions;
using Micro.Net.Dispatch;

namespace Micro.Net.Handling
{
    public class HandlerContext : IHandlerContext
    {
        private readonly IMediator _mediator;

        public HandlerContext(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<TResponse> Dispatch<TRequest, TResponse>(TRequest request, Action<DispatchOptions> ctxAction = null) where TRequest : IContract<TResponse>
        {
            DispatchManagementContext<TRequest, TResponse> context = DispatchManagementContext<TRequest, TResponse>.Create(request);

            ctxAction?.Invoke(context.Options);

            await _mediator.Send(context);

            return context.Response.Payload;
        }

        public async Task Dispatch<TRequest>(TRequest request, Action<DispatchOptions> ctxAction = null) where TRequest : IContract
        {
            DispatchManagementContext<TRequest, ValueTuple> context = DispatchManagementContext<TRequest, ValueTuple>.Create(request);

            ctxAction?.Invoke(context.Options);

            await _mediator.Send(context);
        }
    }

    public interface IHandlerContext
    {
        Task<TResponse> Dispatch<TRequest, TResponse>(TRequest request, Action<DispatchOptions> ctxAction = null) where TRequest : IContract<TResponse>;
        Task Dispatch<TRequest>(TRequest request, Action<DispatchOptions> ctxAction = null) where TRequest : IContract;
    }
}