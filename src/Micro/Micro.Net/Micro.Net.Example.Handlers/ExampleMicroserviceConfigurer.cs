using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Configuration;
using Micro.Net.Example.Contract;
using Micro.Net.Host.Http;

namespace Micro.Net.Example.Handlers
{
    public class ExampleMicroserviceConfigurer : IMicroserviceConfigurer
    {
        public void ConfigureMicroservice(IMicroserviceConfigurable config)
        {
            config.ConfigureReceivers(r => r.Configure<HttpReceiver>(http => http.UseBaseUris()))
                .ConfigureDispatchers(d => { })
                .ConfigureHandlers(h => 
                    h.UseHandler<ExampleQueryHandler, ExampleQueryRequest, ExampleQueryResponse>()
                        .UseHandler<ExampleCommandHandler, ExampleCommandRequest>()
                );
        }
    }
}