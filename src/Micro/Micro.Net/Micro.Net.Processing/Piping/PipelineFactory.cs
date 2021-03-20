using System;
using System.Collections.Generic;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Processing;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Net.Processing.Piping
{
    public class PipelineFactory<TContext> : IPipelineBuilder<TContext>, IFactory<Pipeline<TContext>>
    {
        private readonly IServiceProvider _provider;

        private readonly ICollection<Func<IPipelineStep<TContext>>> _stepProducers =
            new List<Func<IPipelineStep<TContext>>>();

        public PipelineFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IPipelineBuilder<TContext> AddStep(Func<PipelineDelegate<TContext>, PipelineDelegate<TContext>> middleware)
        {
            _stepProducers.Add(() => new GenericPipeStep<TContext>(middleware));

            return this;
        }

        public IPipelineBuilder<TContext> AddStep(Func<PipelineDelegate<TContext>, IServiceProvider, PipelineDelegate<TContext>> middleware)
        {
            _stepProducers.Add(() => new GenericPipeStep<TContext>((del) => middleware(del, _provider)));

            return this;
        }

        public IPipelineBuilder<TContext> AddStep<TStep>() where TStep : IPipelineStep<TContext>
        {
            _stepProducers.Add(() => _provider.GetService<TStep>());

            return this;
        }

        public Pipeline<TContext> Create()
        {
            return new Pipeline<TContext>(_stepProducers);
        }
    }
}