using System;
using Micro.Net.Abstractions.Storage;
using Micro.Net.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Net.Storage.FileSystem
{
    public static class FileSystemStorageExtensions
    {
        public static MicroConfigurer UseFileSagaPersistence(this MicroConfigurer cfgr, Action<FileSystemSagaPersistenceFactoryConfiguration> cfgAction)
        {
            FileSystemSagaPersistenceFactoryConfiguration config = new FileSystemSagaPersistenceFactoryConfiguration();

            cfgAction(config);

            cfgr.AddComponent(sc => sc.AddSingleton(config));
            cfgr.AddComponent(sc =>
                sc.AddTransient<ISagaPersistenceProviderFactory, FileSystemSagaPersistenceProviderFactory>());
            //This will be replaced under the covers by our shell, but providers should still follow BPs
            cfgr.AddComponent(sc => 
                sc.AddTransient(typeof(ISagaPersistenceProvider<>), typeof(FileSystemSagaPersistenceProvider<>)));

            return cfgr;
        }
    }
}