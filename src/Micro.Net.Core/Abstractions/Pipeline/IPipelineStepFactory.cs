using System.Numerics;
using System.Threading.Tasks;

namespace Micro.Net.Core.Pipeline
{
    public interface IPipelineStepFactory
    {
        BigInteger Priority { get; }
        Task<IPipelineStep<TRequest, TResponse>> Create<TRequest, TResponse>();
    }
}