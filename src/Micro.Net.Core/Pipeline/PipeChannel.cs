using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Micro.Net.Core.Pipeline;
using Micro.Net.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;

namespace Micro.Net.Core.Abstractions.Pipeline
{
    internal class PipeChannel : IPipeChannel
    {
        private readonly IEnumerable<IPipelineTail> _pipeTails;
        private readonly IEnumerable<IPipelineStep> _pipeSteps;
        private readonly IEnumerable<IPipelineStepFactory> _pipelineStepFactories;
        private readonly IEnumerable<IPipelineTailFactory> _pipelineTailFactories;

        private readonly IMemoryCache _pipeCache = new MemoryCache(new MemoryCacheOptions());

        public PipeChannel(IEnumerable<IPipelineTail> pipeTails, IEnumerable<IPipelineStep> pipeSteps, IEnumerable<IPipelineStepFactory> pipelineStepFactories, IEnumerable<IPipelineTailFactory> pipelineTailFactories)
        {
            _pipeTails = pipeTails;
            _pipeSteps = pipeSteps;
            _pipelineStepFactories = pipelineStepFactories.OrderBy(x => x.Priority);
            _pipelineTailFactories = pipelineTailFactories.OrderBy(x => x.Priority);
        }

        public async Task<TResponse> Handle<TRequest, TResponse>(TRequest request)
        {
            IPipelineHead<TRequest, TResponse> pipeHead;

            if(!_pipeCache.TryGetValue((typeof(TRequest),typeof(TResponse)), out pipeHead))
            {
                IPipelineTail<TRequest, TResponse> pipeTail = _pipeTails.OfType<IPipelineTail<TRequest, TResponse>>().SingleOrDefault();

                if (pipeTail == null)
                {
                    foreach (IPipelineTailFactory factory in _pipelineTailFactories)
                    {
                        if (factory.TryCreate(out IPipelineTail<TRequest, TResponse> tail))
                        {
                            pipeTail = tail;

                            break;
                        }
                    }
                }

                if (pipeTail == null)
                {
                    throw new MicroConfigurationException("Pipeline wasn't assembled properly!", 999);
                }

                IEnumerable<IPipelineStep<TRequest, TResponse>> pipeSteps = _pipeSteps.OfType<IPipelineStep<TRequest, TResponse>>();

                pipeSteps = pipeSteps.Union(_pipelineStepFactories.Select(x => x.Create<TRequest, TResponse>().Result));

                pipeHead = new Pipeline<TRequest, TResponse>(pipeTail, pipeSteps);
            }

            return await pipeHead.Execute(request);
        }

        public async Task Handle<TMessage>(TMessage request)
        {
            await Handle<TMessage, ValueTuple>(request);
        }
    }
}