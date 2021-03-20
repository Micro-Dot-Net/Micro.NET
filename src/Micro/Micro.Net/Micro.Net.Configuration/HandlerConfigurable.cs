using System;
using Micro.Net.Abstractions.Configuration;
using Micro.Net.Abstractions.Messages;
using Micro.Net.Abstractions.Messages.Dispatch;

namespace Micro.Net.Configuration
{
    public class HandlerConfigurable : IHandlerConfigurable
    {
        public IHandlerConfigurable UseHandler<THandler, TMessage>() where THandler : IHandler<TMessage> where TMessage : IContract
        {
            throw new NotImplementedException();
        }

        public IHandlerConfigurable UseHandler<THandler, TRequest, TResponse>() where THandler : IHandler<TRequest, TResponse> where TRequest : IContract<TResponse>
        {
            throw new NotImplementedException();
        }
    }
}
