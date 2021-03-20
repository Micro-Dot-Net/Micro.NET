using Micro.Net.Abstractions.Messages;

namespace Micro.Net.Abstractions.Contexts
{
    public abstract class ReceiveContext
    {
        public abstract ReceiveRequestContext Request { get; }
        public abstract ReceiveResponseContext Response { get; }
    }
}