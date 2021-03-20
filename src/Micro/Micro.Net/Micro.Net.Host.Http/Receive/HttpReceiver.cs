using System;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Configuration;
using Micro.Net.Abstractions.Messages;
using Microsoft.AspNetCore.Http;

namespace Micro.Net.Host.Http
{
    public class HttpReceiver : IReceiverConfiguration
    {
        public HttpReceiver UseBaseUris(params string[] uris)
        {
            throw new NotImplementedException();
        }

        public HttpReceiver UseRoute<TRequest, TResponse>(string path, string method = default, IMessageMapper mapper = default) where TRequest : IContract<TResponse>
        {
            throw new NotImplementedException();
        }

        public HttpReceiver UseRoute<TMessage>(string path, string method = default, IMessageMapper mapper = default) where TMessage : IContract
        {
            throw new NotImplementedException();
        }

        public HttpReceiver UseMiddleware(Func<HttpContext, RequestDelegate> middleware)
        {
            throw new NotImplementedException();
        }

        public HttpReceiver UseMiddlewareFactory(Func<IServiceProvider, Func<HttpContext, RequestDelegate>> factory)
        {
            throw new NotImplementedException();
        }

        public HttpReceiver UseMiddleware<TMiddleware>() where TMiddleware : IMiddleware
        {
            throw new NotImplementedException();
        }

        public HttpReceiver UseMiddlewareFactory<TMiddlewareFactory>() where TMiddlewareFactory : IMiddlewareFactory
        {
            throw new NotImplementedException();
        }

        public void UseOptions(string name)
        {

        }
    }


    //public class HttpDispatchRouteGroup
    //{
    //    public HttpDispatchRouteGroup Message<TMessage>(string path) where TMessage : IContract
    //    {

    //    }
    //    public HttpDispatchRouteGroup Request<TRequest, TResponse>(string path) where TRequest : IContract<TResponse>
    //    {

    //    }
    //}
}
