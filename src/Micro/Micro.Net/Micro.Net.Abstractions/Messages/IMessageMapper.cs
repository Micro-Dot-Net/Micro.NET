using Micro.Net.Abstractions.Processing;

namespace Micro.Net.Abstractions.Messages
{
    public interface IMessageMapper
    {
        
    }

    public interface IReceiveMessageMapper : IMessageMapper, IPipelineTerminal<>
    {
        
    }

    public interface IDispatchMessageMapper : IMessageMapper
    {
        
    }
}