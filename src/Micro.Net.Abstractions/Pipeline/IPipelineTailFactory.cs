using System.Numerics;

namespace Micro.Net.Core.Pipeline
{
    public interface IPipelineTailFactory
    {
        BigInteger Priority { get; }
        bool TryCreate<TRequest, TResponse>(out IPipelineTail<TRequest, TResponse> pipeTail);
    }
}