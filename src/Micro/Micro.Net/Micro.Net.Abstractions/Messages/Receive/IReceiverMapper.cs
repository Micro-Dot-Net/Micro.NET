using Micro.Net.Abstractions.Contexts;
using Micro.Net.Abstractions.Processing;

namespace Micro.Net.Abstractions.Messages.Receive
{
    public interface IReceiverMapper<TReceiveContext> : IPipelineTerminal<TReceiveContext> where TReceiveContext : ReceiveContext
    {
        
    }
}