namespace Micro.Net.Receive
{
    public interface IReceivePipeFactory
    {
        ReceiveContextDelegate<TRequest, TResponse> Create<TRequest, TResponse>();
    }
}