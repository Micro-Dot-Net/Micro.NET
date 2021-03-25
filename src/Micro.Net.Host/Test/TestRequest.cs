using System;

namespace Micro.Net
{
    public class TestRequest : IContract<TestResponse>
    {
        public Guid RequestId { get; set; }
    }
}