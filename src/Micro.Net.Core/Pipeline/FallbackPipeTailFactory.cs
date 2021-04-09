using System;
using System.Numerics;
using Micro.Net.Abstractions;
using Micro.Net.Exceptions;
using Micro.Net.Handling;

namespace Micro.Net.Core.Pipeline
{
    public class ContextFallbackPipeTailFactory : IPipelineTailFactory
    {
        private readonly ContextStatus? _fallbackStatus;

        public ContextFallbackPipeTailFactory(MicroSystemConfiguration systemConfig)
        {
            _fallbackStatus = systemConfig.PipelineFailbackStatus;
        }

        public BigInteger Priority { get; } = ulong.MaxValue;

        public bool TryCreate<TRequest, TResponse>(out IPipelineTail<TRequest, TResponse> pipeTail)
        {
            if (!typeof(ContextBase).IsAssignableFrom(typeof(TRequest)))
            {
                pipeTail = default;

                return false;
            }

            pipeTail = new GenericPipeTail<TRequest, TResponse>(request =>
            {
                ContextBase context = request as ContextBase;

                switch (_fallbackStatus)
                {
                    case ContextStatus.Resolved:
                        context?.SetResolve();
                        break;
                    case ContextStatus.Faulted:
                        context?.SetFault(new MicroConfigurationException("Pipeline wasn't able to find an appropriate tail for this context!", 451));
                        break;
                    case ContextStatus.Terminated:
                        context?.SetTerminate("Pipeline wasn't able to find an appropriate tail for this context!");
                        break;
                    case ContextStatus.Live:
                    case null:
                    default:
                        break;
                }

                return default;
            });

            return true;
        }
    }
}