using System;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Net.Abstractions
{
    public interface IContainerConfigurable
    {
        IContainerConfigurable Configure(Action<IServiceCollection> config);
    }
}