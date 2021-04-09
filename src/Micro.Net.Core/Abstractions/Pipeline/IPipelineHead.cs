using System.Threading.Tasks;

namespace Micro.Net.Core.Pipeline
{
    public interface IPipelineHead<TRequest, TResponse> : IPipelineHead
    {
        Task<TResponse> Execute(TRequest context);
    }

    public interface IPipelineHead { }
}