using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Micro.Net.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Net.Transport.Http
{
    public static class HttpTransportDependencyExtensions
    {
        public static MicroConfigurer UseHttpReceiver(this MicroConfigurer cfgr, Action<HttpReceiverConfiguration> rcvrCfg)
        {
            cfgr.AddReceiver<HttpReceiver>();

            HttpReceiverConfiguration config = new HttpReceiverConfiguration() { BaseUris = new List<string>(), PathMaps = new Dictionary<string, (Type, Type)>() };

            rcvrCfg(config);

            cfgr.AddComponent(svc => svc.AddSingleton(config));

            foreach ((Type, Type) rcvbl in config.PathMaps.Values)
            {
                cfgr.AddReceivable(rcvbl.Item1, rcvbl.Item2);
            }

            cfgr.AddComponent<HttpListener>(ServiceLifetime.Transient);

            return cfgr;
        }

        public static MicroConfigurer UseHttpDispatcher(this MicroConfigurer cfgr, Action<HttpDispatcherConfiguration> dsptCfg)
        {
            cfgr.AddDispatcher<HttpDispatcher>();

            HttpDispatcherConfiguration config = new HttpDispatcherConfiguration() { DefaultHeaders = new Dictionary<string, string[]>(), Routes = new Dictionary<(Type, Type), (Uri, HttpMethod)>() };

            dsptCfg(config);

            cfgr.AddComponent(svc => svc.AddSingleton(config));

            foreach ((Type, Type) dispatchable in config.Routes.Keys)
            {
                cfgr.AddDispatchable(dispatchable.Item1, dispatchable.Item2);
            }

            cfgr.AddComponent<HttpClient>(ServiceLifetime.Transient);

            return cfgr;
        }
    }
}
