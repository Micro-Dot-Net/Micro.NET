using Micro.Net.Abstractions;

namespace Micro.Net.Example.Contract
{
    public class ExampleCommandRequest : IContract
    {
        public int ShiftValue { get; set; }
    }
}