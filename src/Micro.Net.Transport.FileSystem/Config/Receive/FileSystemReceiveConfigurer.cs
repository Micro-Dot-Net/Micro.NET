using System;
using System.Collections.Generic;
using Micro.Net.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Net.Transport.FileSystem
{
    public class FileSystemReceiveConfigurer
    {
        internal FileSystemReceiveConfigurer() { }

        private FileSystemReceiveConfiguration _config = new FileSystemReceiveConfiguration(){Mappings = new Dictionary<Type, MessageProcessConfiguration>()};

        public FileSystemReceiveConfigurer Handle<TMessage>(string receiveDir, Type receiveSerializer, bool keepProcessed = false, bool keepSkip = true, string skipDir = null, string processedDir = null, string requestFilter = null) where TMessage : IContract
        {
            _config.Mappings[typeof(TMessage)] = new MessageProcessConfiguration()
            {
                RequestDir = receiveDir,
                RequestSerializer = receiveSerializer.AssemblyQualifiedName,
                KeepProcessed = keepProcessed,
                KeepSkips = keepSkip,
                ProcessedDirectory = processedDir ?? string.Empty,
                SkipDirectory = skipDir ?? string.Empty,
                RequestFilter = requestFilter ?? string.Empty,
                ResponseDir = string.Empty,
                ResponseSerializer = string.Empty,
                ResponseType = typeof(ValueTuple)
            };

            return this;
        }

        public FileSystemReceiveConfigurer Handle<TRequest, TResponse>(string receiveDir, Type receiveSerializer, bool keepProcessed = false, bool keepSkip = true, string skipDir = null, string processedDir = null, string responseDir = null, Type responseSerializer = null, string requestFilter = null) where TRequest : IContract<TResponse>
        {
            _config.Mappings[typeof(TRequest)] = new MessageProcessConfiguration()
            {
                RequestDir = receiveDir,
                RequestSerializer = receiveSerializer.AssemblyQualifiedName,
                KeepProcessed = keepProcessed,
                KeepSkips = keepSkip,
                ProcessedDirectory = processedDir ?? string.Empty,
                SkipDirectory = skipDir ?? string.Empty,
                RequestFilter = requestFilter ?? string.Empty,
                ResponseDir = responseDir ?? string.Empty,
                ResponseSerializer = responseSerializer?.AssemblyQualifiedName ?? string.Empty,
                ResponseType = typeof(TResponse)
            };

            return this;
        }

        internal FileSystemReceiveConfiguration produce()
        {
            return _config;
        }
    }
}
