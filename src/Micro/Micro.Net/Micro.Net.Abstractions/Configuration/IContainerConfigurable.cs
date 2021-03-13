using System;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Net.Abstractions.Configuration
{
    public interface IContainerConfigurable
    {
        IContainerConfigurable Configure(Action<IServiceCollection> config);
    }
}