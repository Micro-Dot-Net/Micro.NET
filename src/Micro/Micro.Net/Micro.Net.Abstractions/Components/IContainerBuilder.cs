using Microsoft.Extensions.DependencyInjection;

namespace Micro.Net.Abstractions.Components
{
    public interface IContainerBuilder
    {
        void Configure(IServiceCollection collection);
    }
}