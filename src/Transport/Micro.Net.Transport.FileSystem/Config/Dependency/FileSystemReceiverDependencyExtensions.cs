using System;
using Micro.Net.Abstractions;
using Micro.Net.Dispatch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Net.Transport.FileSystem
{
    public static partial class FileSystemDependencyExtensions
    {
        public static IServiceCollection UseFileReceiver(this IServiceCollection collection, Action<FileSystemReceiveConfigurer> config)
        {
            collection.AddSingleton<IStartable, FileSystemReceiver>();
            collection.AddSingleton<IStoppable, FileSystemReceiver>();

            FileSystemReceiveConfigurer cfgr = new FileSystemReceiveConfigurer();

            config(cfgr);

            collection.AddSingleton<FileSystemReceiveConfiguration>(cfgr.produce());

            return collection;
        }

        public static IServiceCollection UseFileReceiver(this IServiceCollection collection, string configSectionName)
        {
            collection.AddSingleton<IStartable, FileSystemReceiver>();
            collection.AddSingleton<IStoppable, FileSystemReceiver>();

            collection.AddSingleton<FileSystemReceiveConfiguration>(sp => 
                sp.GetRequiredService<IConfiguration>().GetSection(configSectionName).Get<FileSystemReceiveConfiguration>());

            return collection;
        }
    }

    
}