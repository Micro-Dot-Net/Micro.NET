using System;
using System.Collections.Generic;
using Micro.Net.Abstractions.Sagas;
using Micro.Net.Core.Extensions;

namespace Micro.Net.Storage.FileSystem
{
    public class FileSystemSagaPersistenceFactoryConfiguration
    {
        internal Dictionary<Type, FileSystemSagaPersistenceConfiguration> _mappedConfigs =
            new Dictionary<Type, FileSystemSagaPersistenceConfiguration>();

        internal FileSystemSagaPersistenceConfiguration _defaultConfig = new FileSystemSagaPersistenceConfiguration();

        public FileSystemSagaPersistenceFactoryConfiguration AddConfig<TData>(Action<FileSystemSagaPersistenceConfiguration> action, bool overwrite = false) where TData : class, ISagaData
        {
            FileSystemSagaPersistenceConfiguration config;

            if (overwrite || (config = _mappedConfigs[typeof(TData)]) == null)
            {
                config = new FileSystemSagaPersistenceConfiguration();
            }
            else
            {
                config = config.Copy();
            }

            action?.Invoke(config);

            _validateConfiguration(config, false);

            _mappedConfigs[typeof(TData)] = config;

            return this;
        }

        public FileSystemSagaPersistenceFactoryConfiguration SetDefaults(Action<FileSystemSagaPersistenceConfiguration> action)
        {
            FileSystemSagaPersistenceConfiguration defaultConfig = _defaultConfig.Copy();

            action?.Invoke(defaultConfig);

            _validateConfiguration(defaultConfig, true);

            _defaultConfig = defaultConfig;

            return this;
        }

        private void _validateConfiguration(FileSystemSagaPersistenceConfiguration config, bool isDefault)
        {

        }
    }
}