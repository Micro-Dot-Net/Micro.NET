using System;
using Micro.Net.Abstractions;

namespace Micro.Net.Handling
{
    public interface IMicroSystemConfiguration
    {
        IMicroSystemDispatchConfiguration Dispatch { get; set; }
        Type DefaultSerializer { get; }
        ContextStatus? PipelineFailbackStatus { get; set; }
        IMicroSystemConfiguration SetDefaultSerializer<TSerializer>() where TSerializer : class, ISerializer;
    }
}