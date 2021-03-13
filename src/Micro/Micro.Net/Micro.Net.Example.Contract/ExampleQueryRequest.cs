using System;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Messages;

namespace Micro.Net.Example.Contract
{
    public class ExampleQueryRequest : IContract<ExampleQueryResponse>
    {
        public string Token { get; set; }
    }
}
