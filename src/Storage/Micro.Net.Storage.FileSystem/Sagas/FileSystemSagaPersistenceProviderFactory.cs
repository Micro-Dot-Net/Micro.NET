using System;
using System.Collections.Generic;
using System.IO;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Sagas;
using Micro.Net.Abstractions.Storage;
using Micro.Net.Core.Extensions;
using Micro.Net.Exceptions;
using SmartFormat;

namespace Micro.Net.Storage.FileSystem
{
    public class FileSystemSagaPersistenceProviderFactory : ISagaPersistenceProviderFactory
    {
        private readonly FileSystemSagaPersistenceFactoryConfiguration _config;
        private readonly ISerializerCollection _serializerCollection;

        public FileSystemSagaPersistenceProviderFactory(FileSystemSagaPersistenceFactoryConfiguration config, ISerializerCollection serializerCollection)
        {
            _config = config;
            _serializerCollection = serializerCollection;
        }

        private void assertDirectoriesExist(FileSystemSagaPersistenceConfiguration config)
        {
            Directory.CreateDirectory(config.StoragePath);
            //Directory.CreateDirectory(config.LogPath);

            if (config.KeepProcessed)
            {
                Directory.CreateDirectory(config.ProcessedPath);
            }

            //if (config.KeepCorrupted)
            //{
            //    Directory.CreateDirectory(config.CorruptedPath);
            //}
        }

        private void interpolateDirectories(FileSystemSagaPersistenceConfiguration config, Type dataType)
        {
            config.StoragePath = Smart.Format(config.StoragePath, new {SagaData = dataType.Name});
            //config.LogPath = Smart.Format(config.LogPath, new { SagaData = dataType.Name });

            if (config.KeepProcessed)
            {
                config.ProcessedPath = Smart.Format(config.ProcessedPath, new { SagaData = dataType.Name });
            }

            //if (config.KeepCorrupted)
            //{
            //    config.CorruptedPath = Smart.Format(config.CorruptedPath, new { SagaData = dataType.Name });
            //}
        }

        public ISagaPersistenceProvider<TData> Create<TData>() where TData : class, ISagaData
        {
            FileSystemSagaPersistenceConfiguration config =
                _config._mappedConfigs[typeof(TData)] ?? _config._defaultConfig;

            config = config.Copy();

            if (_serializerCollection.Get(config.Serializer) == null)
            {
                throw MicroConfigurationException.MissingRegistrations(new Dictionary<string, string>{ { config.Serializer, "ISerializer" } });
            }

            interpolateDirectories(config, typeof(TData));

            assertDirectoriesExist(config);

            FileSystemSagaPersistenceProvider<TData> provider = new FileSystemSagaPersistenceProvider<TData>(config, _serializerCollection);

            return provider;
        }
    }
}