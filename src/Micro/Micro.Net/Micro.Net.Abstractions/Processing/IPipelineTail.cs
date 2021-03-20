using System;
using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Processing
{
    public class PipelineTail<TContext> : IPipelineTerminal<TContext>
    {
        event Action<TContext> PipelineComplete;
        public async Task Step(TContext context, PipelineDelegate<TContext> next)
        {
            PipelineComplete?.Invoke(context);
        }
    }

    public interface IPipelineTerminal<TContext> : IPipelineStep<TContext>
    {
        
    }
}