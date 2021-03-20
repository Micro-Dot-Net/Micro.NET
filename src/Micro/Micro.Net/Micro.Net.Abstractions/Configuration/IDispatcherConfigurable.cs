using System;
using Micro.Net.Abstractions.Messages.Dispatch;

namespace Micro.Net.Abstractions.Configuration
{
    public interface IDispatcherConfigurable
    {
        IDispatcherConfigurable Configure<TDispatcher>(Action<TDispatcher> config) where TDispatcher : IDispatcherConfiguration;
    }
}