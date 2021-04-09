using System;
using System.Threading.Tasks;

namespace Micro.Net.Core.Pipeline
{
    public class GenericPipeStep<TRequest, TResponse> : IPipelineStep<TRequest, TResponse>
    {
        private readonly Func<PipelineDelegate<TRequest, TResponse>, PipelineDelegate<TRequest, TResponse>> _middleware;

        public GenericPipeStep(Func<PipelineDelegate<TRequest, TResponse>, PipelineDelegate<TRequest, TResponse>> middleware)
        {
            _middleware = middleware;
        }

        public async Task<TResponse> Step(TRequest context, PipelineDelegate<TRequest, TResponse> next)
        {
            return await _middleware.Invoke(next).Invoke(context);
        }
    }
}