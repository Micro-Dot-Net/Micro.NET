using System;
using Micro.Net.Abstractions;

namespace Micro.Net.Example.Contract
{
    public class ExampleQueryRequest : IContract<ExampleQueryResponse>
    {
        public string Token { get; set; }
    }
}
