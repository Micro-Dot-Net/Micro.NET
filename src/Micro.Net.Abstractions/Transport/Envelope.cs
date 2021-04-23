using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Micro.Net.Abstractions.Transport
{
    public class Envelope
    {
        internal Envelope(Type messageType)
        {
            MessageType = messageType;
        }

        [IgnoreDataMember]
        public Type MessageType { get; }

        public Dictionary<string, string[]> Headers { get; set; }
    }

    public class Envelope<TValue> : Envelope
    {
        public Envelope() : base(typeof(TValue))
        {

        }

        public TValue Message { get; set; } = default;
    }
}