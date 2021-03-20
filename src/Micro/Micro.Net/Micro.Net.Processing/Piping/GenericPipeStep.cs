using System;
using System.Threading.Tasks;
using Micro.Net.Abstractions.Processing;

namespace Micro.Net.Processing.Piping
{
    public class GenericPipeStep<TContext> : IPipelineStep<TContext>
    {
        private readonly Func<PipelineDelegate<TContext>, PipelineDelegate<TContext>> _middleware;

        public GenericPipeStep(Func<PipelineDelegate<TContext>, PipelineDelegate<TContext>> middleware)
        {
            _middleware = middleware;
        }

        public async Task Step(TContext context, PipelineDelegate<TContext> next)
        {
            await _middleware.Invoke(next).Invoke(context);
        }
    }
}