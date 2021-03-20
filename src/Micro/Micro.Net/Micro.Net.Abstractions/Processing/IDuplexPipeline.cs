namespace Micro.Net.Abstractions.Processing
{
    public interface IDuplexPipeline<TRequest, TResponse> : IPipelineHead<TRequest>, IPipelineTerminal<TResponse>
    {
        
    }
}