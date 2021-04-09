using System;
using System.Collections.Generic;
using System.Text;
using Micro.Net.Core.Abstractions.Pipeline;
using Micro.Net.Receive;

namespace Micro.Net.Core.Receive
{
    public class ReceivePipeFactory : IReceivePipeFactory
    {
        private readonly IPipeChannel _channel;


        public ReceivePipeFactory(IPipeChannel channel)
        {
            _channel = channel;
        }

        public ReceiveContextDelegate<TRequest, TResponse> Create<TRequest, TResponse>()
        {
            return async (ReceiveContext<TRequest, TResponse> context) =>
            {
                await _channel.Handle(context);
            };
        }
    }
}
