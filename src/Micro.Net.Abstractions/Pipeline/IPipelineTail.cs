using System.Threading.Tasks;

namespace Micro.Net.Core.Pipeline
{
    public interface IPipelineTail<TRequest, TResponse> : IPipelineTail
    {
        Task<TResponse> Handle(TRequest request);
    }

    public interface IPipelineTail { }
}