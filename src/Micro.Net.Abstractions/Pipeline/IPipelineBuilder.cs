using System;

namespace Micro.Net.Core.Pipeline
{
    public interface IPipelineBuilder<TRequest, TResponse>
    {
        IPipelineBuilder<TRequest, TResponse> AddStep(Func<PipelineDelegate<TRequest, TResponse>, PipelineDelegate<TRequest, TResponse>> middleware);
        IPipelineBuilder<TRequest, TResponse> AddStep<TStep>() where TStep : IPipelineStep<TRequest, TResponse>;
    }
}