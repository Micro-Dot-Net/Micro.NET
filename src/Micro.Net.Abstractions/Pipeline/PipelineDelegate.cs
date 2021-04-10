using System.Threading.Tasks;

namespace Micro.Net.Core.Pipeline
{
    public delegate Task<TResponse> PipelineDelegate<TRequest, TResponse>(TRequest request);
}