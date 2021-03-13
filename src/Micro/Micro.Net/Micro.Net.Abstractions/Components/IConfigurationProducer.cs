using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Net.Abstractions.Components
{
    public interface IConfigurationProducer
    {
        void Configure(IServiceCollection collection, IConfiguration configuration);
    }
}