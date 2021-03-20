using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Processing
{
    public interface IPipelineStep<TContext>
    {
        Task Step(TContext context, PipelineDelegate<TContext> next);
    }
}