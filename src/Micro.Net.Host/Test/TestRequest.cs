using System;
using Micro.Net.Abstractions;

namespace Micro.Net.Test
{
    public class TestRequest : IContract<TestResponse>
    {
        public Guid RequestId { get; set; }
    }
}