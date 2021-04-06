using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Net.Core.Configuration
{
    public static class MicroConfigurationExtensions
    {
        public static IServiceCollection UseMicroNet(this IServiceCollection collection, Action<MicroConfigurer> configureAction)
        {
            MicroConfigurer cfgr = new MicroConfigurer();

            configureAction(cfgr);

            cfgr.populate(collection);

            return collection;
        }
    }
}
