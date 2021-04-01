using System;
using System.Collections.Generic;
using Micro.Net.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Net.Transport.FileSystem
{
    public class FileSystemDispatchConfigurer
    {
        internal FileSystemDispatchConfigurer()
        {

        }

        private FileSystemDispatchConfiguration _config = new FileSystemDispatchConfiguration() { Mappings = new Dictionary<Type, MessageProcessConfiguration>() };

        public FileSystemDispatchConfigurer Route<TMessage>(string requestDir, Type requestSerializer) where TMessage : IContract
        {
            _config.Mappings[typeof(TMessage)] = new MessageProcessConfiguration()
            {
                RequestDir = requestDir,
                RequestSerializer = requestSerializer.AssemblyQualifiedName,
                KeepProcessed = false,
                KeepSkips = false,
                ProcessedDirectory = string.Empty,
                SkipDirectory = string.Empty,
                RequestFilter = string.Empty,
                ResponseDir = string.Empty,
                ResponseSerializer = string.Empty,
                ResponseType = typeof(ValueTuple)
            };

            return this;
        }

        public FileSystemDispatchConfigurer Route<TRequest, TResponse>(string requestDir, Type requestSerializer) where TRequest : IContract<TResponse>
        {
            _config.Mappings[typeof(TRequest)] = new MessageProcessConfiguration()
            {
                RequestDir = requestDir,
                RequestSerializer = requestSerializer.AssemblyQualifiedName,
                KeepProcessed = false,
                KeepSkips = false,
                ProcessedDirectory = string.Empty,
                SkipDirectory = string.Empty,
                RequestFilter = string.Empty,
                ResponseDir = string.Empty,
                ResponseSerializer = string.Empty,
                ResponseType = typeof(TResponse)
            };

            return this;
        }

        internal FileSystemDispatchConfiguration produce()
        {
            return _config;
        }
    }
    
}