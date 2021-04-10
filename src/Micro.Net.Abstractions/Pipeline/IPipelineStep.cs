using System.Threading.Tasks;

namespace Micro.Net.Core.Pipeline
{
    public interface IPipelineStep<TRequest, TResponse> : IPipelineStep
    {
        Task<TResponse> Step(TRequest context, PipelineDelegate<TRequest, TResponse> next);
    }

    public interface IPipelineStep { }
}