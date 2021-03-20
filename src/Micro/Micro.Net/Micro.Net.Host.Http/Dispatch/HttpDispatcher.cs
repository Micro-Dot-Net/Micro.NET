using System;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Configuration;
using Micro.Net.Abstractions.Messages;

namespace Micro.Net.Host.Http
{
    public class HttpDispatcher : IDispatcherConfiguration
    {
        public HttpDispatcher UseOptions(string name)
        {
            throw new NotImplementedException();
        }

        public HttpDispatcher Direct<TRequest, TResponse>(Action<HttpDispatchRouteConfigurable> config) where TRequest : IContract<TResponse>
        {
            throw new NotImplementedException();
        }

        public HttpDispatcher Direct<TMessage>(Action<HttpDispatchRouteConfigurable> config) where TMessage : IContract
        {
            throw new NotImplementedException();
        }

        //public HttpDispatcher Direct(Action<HttpDispatchRouteGroup> group, Action<HttpDispatchRouteConfigurable> config)
        //{

        //}
    }
}