using System;
using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Micro.Net.Example.Contract;
using Micro.Net.Example.Services;

namespace Micro.Net.Example.Handlers
{
    public class ExampleQueryHandler : IHandler<ExampleQueryRequest, ExampleQueryResponse>
    {
        private readonly IValueHolderService _valueService;

        public ExampleQueryHandler(IValueHolderService valueService)
        {
            _valueService = valueService;
        }

        public async Task<ExampleQueryResponse> Handle(ExampleQueryRequest request, IHandlerContext context)
        {
            ExampleQueryResponse response = new ExampleQueryResponse(){ Token = request.Token, Value = _valueService.ReadValue() };

            await Task.Yield();

            return response;
        }
    }

    public class ExampleMicroserviceConfigurer : IMicroserviceConfigurer
    {
        public void ConfigureMicroservice(IMicroserviceConfigurable config)
        {
            config.ConfigureReceivers(r => { })
                .ConfigureDispatchers(d => { })
                .ConfigureHandlers(h => 
                    h.UseHandler<ExampleQueryHandler, ExampleQueryRequest, ExampleQueryResponse>()
                        .UseHandler<ExampleCommandHandler, ExampleCommandRequest>()
                    );
        }
    }
}
