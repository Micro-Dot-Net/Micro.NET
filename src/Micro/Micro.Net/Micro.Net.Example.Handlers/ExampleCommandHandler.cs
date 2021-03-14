using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Messages;
using Micro.Net.Example.Contract;
using Micro.Net.Example.Services;

namespace Micro.Net.Example.Handlers
{
    public class ExampleCommandHandler : IHandler<ExampleCommandRequest>
    {
        private readonly IValueHolderService _valueService;

        public ExampleCommandHandler(IValueHolderService valueService)
        {
            _valueService = valueService;
        }

        public async Task Handle(ExampleCommandRequest message, IHandlerContext context)
        {
            _valueService.ShiftValue(message.ShiftValue);
        }
    }
}