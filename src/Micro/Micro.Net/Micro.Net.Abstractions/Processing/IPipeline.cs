namespace Micro.Net.Abstractions.Processing
{
    public interface IPipeline<TContext> : IPipelineHead<TContext>, IPipelineTail<TContext>
    {
        
    }
}