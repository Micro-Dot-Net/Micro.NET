using System.Threading.Tasks;

namespace Micro.Net.Receive
{
    public delegate Task ReceiveContextDelegate<TRequest, TResponse>(ReceiveContext<TRequest, TResponse> context);
}