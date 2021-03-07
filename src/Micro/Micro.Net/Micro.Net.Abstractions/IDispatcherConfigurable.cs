using System;

namespace Micro.Net.Abstractions
{
    public interface IDispatcherConfigurable
    {
        IDispatcherConfigurable Configure<TDispatcher>(Action<TDispatcher> config) where TDispatcher : DispatcherService;
    }
}