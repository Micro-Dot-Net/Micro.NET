using System;
using Microsoft.AspNetCore.Http;

namespace Micro.Net.Host.Http
{
    public class RequestMessageMapper : IMessageMapper
    {
        private readonly Func<Type, HttpRequest, object> _delegate;

        private RequestMessageMapper(Func<Type, HttpRequest, object> @delegate)
        {
            _delegate = @delegate;
        }

        public object Map(Type messageType, HttpRequest context)
        {
            return _delegate(messageType, context);
        }

        public static RequestMessageMapper FromQuery => new RequestMessageMapper();
        public static RequestMessageMapper FromBody => new RequestMessageMapper();
    }
    
    
}