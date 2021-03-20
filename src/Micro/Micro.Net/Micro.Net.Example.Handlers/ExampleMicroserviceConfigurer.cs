using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Configuration;
using Micro.Net.Example.Contract;
using Micro.Net.Host.Discovery.Configuration;
using Micro.Net.Host.Http;
using Microsoft.AspNetCore.Http;

namespace Micro.Net.Example.Handlers
{
    public class ExampleMicroserviceConfigurer : IMicroserviceConfigurer
    {
        public void ConfigureMicroservice(IMicroserviceConfigurable config)
        {
            config.ConfigureReceivers(r => r.Configure<HttpReceiver>(http => http.UseBaseUris()))
                .ConfigureDispatchers(d =>
                {
                    d.Configure<HttpDispatcher>(http =>
                    {
                        http.Direct<ExampleDispatchableCommand>(rt =>
                        {
                            rt.UseMapper(RequestMessageMapper.FromBody).UseMethod(HttpMethods.Post).UseUri("");
                        });
                        http.Direct<ExampleDispatchableRequest, ExampleDispatchableResponse>(rt =>
                        {
                            rt.UseMapper(RequestMessageMapper.FromQuery).UseMethod(HttpMethods.Post).UseUri("");
                        });
                    });
                })
                .ConfigureHandlers(h => 
                    h.UseHandler<ExampleQueryHandler, ExampleQueryRequest, ExampleQueryResponse>()
                        .UseHandler<ExampleCommandHandler, ExampleCommandRequest>()
                )
                .UseDiscovery<ConfigurationDiscovery>(d => d.FromSection("services"));
        }
    }
}