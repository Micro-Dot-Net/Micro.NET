using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Micro.Net.Core.Abstractions.Pipeline;
using Newtonsoft.Json;

namespace Micro.Net.Core.Pipeline
{
    public class Pipeline<TRequest, TResponse> : IPipelineHead<TRequest, TResponse>
	{
        private readonly IPipelineTail<TRequest, TResponse> _tail;
        private readonly IEnumerable<IPipelineStep<TRequest, TResponse>> _steps;

        public Pipeline(IPipelineTail<TRequest, TResponse> tail, IEnumerable<IPipelineStep<TRequest, TResponse>> steps)
        {
            _tail = tail;
            _steps = steps;
        }

		public Task<TResponse> Execute(TRequest context)
		{
			PipelineDelegate<TRequest, TResponse> pipe = build(_tail);

			return pipe.Invoke(context);
		}

		private PipelineDelegate<TRequest, TResponse> build(IPipelineTail<TRequest,TResponse> tail)
		{
			PipelineDelegate<TRequest, TResponse> pipe = async request => await tail.Handle(request);

			for (int idx = _steps.Count() - 1; idx >= 0; idx--)
			{
				PipelineDelegate<TRequest, TResponse> lpipe = pipe;

				IPipelineStep<TRequest, TResponse> step = _steps.ElementAt(idx);

				pipe = (ctx) => step.Step(ctx, lpipe);
			}

			return pipe;
		}
	}

	public class LoggingPipeStepFactory : IPipelineStepFactory
    {
		public async Task<IPipelineStep<TRequest, TResponse>> Create<TRequest, TResponse>()
        {
            return new GenericPipeStep<TRequest, TResponse>(next => async req =>
            {
                Console.WriteLine(JsonConvert.SerializeObject(req, Formatting.Indented));

                return await next.Invoke(req);
            });
        }
	}

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
