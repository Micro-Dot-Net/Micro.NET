using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Micro.Net.Receive;

namespace Micro.Net.Core.Receive
{
    public class ReceivePipeFactory : IReceivePipeFactory
    {
        private readonly IMediator _mediator;


        public ReceivePipeFactory(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ReceiveContextDelegate<TRequest, TResponse> Create<TRequest, TResponse>()
        {
            return async (ReceiveContext<TRequest, TResponse> context) =>
            {
                await _mediator.Send<Unit>(context);
            };
        }
    }
}
