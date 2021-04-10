using System.Threading.Tasks;

namespace Micro.Net.Receive
{
    public delegate Task ReceiveContextDelegate<TRequest, TResponse>(IReceiveContext<TRequest, TResponse> context);
}