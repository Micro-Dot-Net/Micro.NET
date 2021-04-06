using System;
using System.Collections.Generic;
using System.Linq;
using Micro.Net.Abstractions;
using Micro.Net.Exceptions;
using Micro.Net.Handling;

namespace Micro.Net.Serializing
{
    internal class SerializerCollection : ISerializerCollection
    {
        private readonly IEnumerable<ISerializer> _serializers;
        private readonly ISerializer _default;

        public SerializerCollection(IEnumerable<ISerializer> serializers, MicroSystemConfiguration systemConfig)
        {
            _serializers = serializers;
            _default = serializers.FirstOrDefault(x => x.GetType() == systemConfig.DefaultSerializer) ??
                       throw MicroConfigurationException.MissingRegistrations(new Dictionary<string, string>
                           {{systemConfig.DefaultSerializer.FullName, nameof(ISerializer)}});
        }

        public ISerializer Default => _default;

        public ISerializer Get(string name)
        {
            return _serializers.FirstOrDefault(x => 
                String.Equals(x.GetType().Name, name, StringComparison.CurrentCultureIgnoreCase) 
                || String.Equals(x.GetType().FullName, name, StringComparison.CurrentCultureIgnoreCase)
            ) ?? Default;
        }

        public bool TryGet(string name, out ISerializer serializer)
        {
            serializer = _serializers.FirstOrDefault(x =>
                String.Equals(x.GetType().Name, name, StringComparison.CurrentCultureIgnoreCase)
                || String.Equals(x.GetType().FullName, name, StringComparison.CurrentCultureIgnoreCase)
            );

            return serializer != null;
        }
    }
}