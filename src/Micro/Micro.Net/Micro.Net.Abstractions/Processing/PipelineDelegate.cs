using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Processing
{
    public delegate Task PipelineDelegate<TContext>(TContext context);
}