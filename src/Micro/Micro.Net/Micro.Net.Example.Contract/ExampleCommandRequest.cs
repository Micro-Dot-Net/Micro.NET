using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Messages;

namespace Micro.Net.Example.Contract
{
    public class ExampleCommandRequest : IContract
    {
        public int ShiftValue { get; set; }
    }
}