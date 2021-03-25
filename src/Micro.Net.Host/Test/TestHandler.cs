using System;
using System.Threading.Tasks;

namespace Micro.Net
{
    public class TestHandler : IHandle<TestRequest,TestResponse>
    {
        public async Task<TestResponse> Handle(TestRequest message, HandlerContext context)
        {
            TestResponse response = new TestResponse()
            {
                RequestId = message.RequestId,
                ResponseId = Guid.NewGuid()
            };

            await context.Dispatch(new TestCommand() {Id = message.RequestId});

            return response;
        }
    }
}