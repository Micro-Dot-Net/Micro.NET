using System;
using Micro.Net.Abstractions.Configuration;

namespace Micro.Net.Configuration
{
    public class DispatcherConfigurable : IDispatcherConfigurable
    {
        public IDispatcherConfigurable Configure<TDispatcher>(Action<TDispatcher> config) where TDispatcher : IDispatcherConfiguration
        {
            throw new NotImplementedException();
        }
    }
}