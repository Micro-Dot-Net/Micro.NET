using System;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Configuration;
using Micro.Net.Abstractions.Messages;

namespace Micro.Net.Host.Http
{
    public class HttpDispatcher : IDispatcherConfiguration
    {
        public void UseOptions()
        {

        }

        public HttpDispatcher Direct<TRequest, TResponse>(Action<HttpDispatchRouteConfigurable> config) where TRequest : IContract<TResponse>
        {

        }

        public HttpDispatcher Direct<TMessage>(Action<HttpDispatchRouteConfigurable> config) where TMessage : IContract
        {

        }

        //public HttpDispatcher Direct(Action<HttpDispatchRouteGroup> group, Action<HttpDispatchRouteConfigurable> config)
        //{

        //}
    }

    public class RemoteEndpointOptions
    {

    }

    public class RemoteDispatcherEndpointOptions : RemoteEndpointOptions
    {

    }

    public class RemoteReceiverEndpointOptions : RemoteEndpointOptions
    {

    }
}