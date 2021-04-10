using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Micro.Net.Dispatch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Net.Transport.FileSystem.Config
{
    public static partial class FileSystemDependencyExtensions
    {
        public static IServiceCollection UseFileDispatcher(this IServiceCollection collection, string configSectionName)
        {
            collection.AddSingleton<IDispatcher, FileSystemDispatcher>();

            collection.AddSingleton<FileSystemDispatchConfiguration>(sp =>
                sp.GetRequiredService<IConfiguration>().GetSection(configSectionName).Get<FileSystemDispatchConfiguration>());

            return collection;
        }

        public static IServiceCollection UseFileDispatcher(this IServiceCollection collection, Action<FileSystemDispatchConfigurer> config)
        {
            collection.AddSingleton<IDispatcher, FileSystemDispatcher>();

            FileSystemDispatchConfigurer cfgr = new FileSystemDispatchConfigurer();

            config(cfgr);

            collection.AddSingleton<FileSystemDispatchConfiguration>(cfgr.produce());

            return collection;
        }
    }
}
