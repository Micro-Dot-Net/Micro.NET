using System;
using System.Collections.Generic;
using Micro.Net.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Net.Transport.Feather
{

    public static class FeatherTransportExtensions
    {
        public static IMicroConfigurer UseFeatherHttpReceiver(this IMicroConfigurer configurer, Action<FeatherReceiverConfiguration> cfgAction)
        {
            FeatherReceiverConfiguration config = new FeatherReceiverConfiguration()
            {
                BaseUris = new List<string>(),
                PathMaps = new Dictionary<(string path, System.Net.Http.HttpMethod verb), (Type, Type)>()
            };

            cfgAction(config);

            configurer.AddComponent(sc => sc.AddSingleton(config));
            configurer.AddReceiver<FeatherReceiver>();

            foreach ((Type, Type) rcvbl in config.PathMaps.Values)
            {
                configurer.AddReceivable(rcvbl.Item1, rcvbl.Item2);
            }

            return configurer;
        }
    }
}