using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Micro.Net.Abstractions.Processing
{
    public interface IPipelineBuilder<TContext>
    {
        IPipelineBuilder<TContext> AddStep(Func<PipelineDelegate<TContext>, PipelineDelegate<TContext>> middleware);
        IPipelineBuilder<TContext> AddStep<TStep>() where TStep : IPipelineStep<TContext>;
    }
}
