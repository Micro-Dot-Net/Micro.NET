using System.Threading.Tasks;

namespace Micro.Net.Abstractions.Processing
{
    public interface IPipelineHead<TContext>
    {
        Task Execute(TContext context);
    }
}