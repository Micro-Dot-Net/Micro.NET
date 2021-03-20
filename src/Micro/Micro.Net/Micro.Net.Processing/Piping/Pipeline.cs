using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Micro.Net.Abstractions.Processing;

namespace Micro.Net.Processing.Piping
{
    public class Pipeline<TContext> : IPipelineHead<TContext>
    {
        private readonly ICollection<Func<IPipelineStep<TContext>>> _producers;

        public Pipeline(ICollection<Func<IPipelineStep<TContext>>> producers)
        {
            _producers = producers;
        }

        public async Task Execute(TContext context)
        {
            PipelineDelegate<TContext> pipe = build();

            await pipe.Invoke(context);
        }

        public event Action<TContext> PipelineComplete;

        private PipelineDelegate<TContext> build()
        {
            PipelineDelegate<TContext> pipe = async context =>
            {
                PipelineComplete?.Invoke(context);
            };

            for (int idx = _producers.Count - 1; idx >= 0; idx--)
            {
                PipelineDelegate<TContext> lpipe = pipe;

                IPipelineStep<TContext> step = _producers.ElementAt(idx)();

                pipe = (ctx) => step.Step(ctx, lpipe);
            }

            return pipe;
        }
    }
}
