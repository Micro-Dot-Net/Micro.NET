using System;
using System.Collections.Generic;
using System.Linq;
using Micro.Net.Abstractions;
using Micro.Net.Serializing;

namespace Micro.Net.Handling
{
    public class MicroSystemConfiguration
    {
        internal MicroSystemConfiguration()
        {
            Dispatch = new MicroSystemDispatchConfiguration();
            DefaultSerializer = typeof(JsonSerializer);
            PipelineFailbackStatus = ContextStatus.Faulted;
        }

        public MicroSystemDispatchConfiguration Dispatch { get; set; }
        public Type DefaultSerializer { get; private set; }
        public ContextStatus? PipelineFailbackStatus { get; set; }

        internal bool TryValidate(out IEnumerable<Exception> exceptions)
        {
            List<Exception> _ex = new List<Exception>();

            

            exceptions = _ex;

            return !_ex.Any();
        }

        public MicroSystemConfiguration SetDefaultSerializer<TSerializer>() where TSerializer : class, ISerializer
        {
            DefaultSerializer = typeof(TSerializer);

            return this;
        }
    }
}