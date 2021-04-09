using System;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Net.Core.Abstractions.Pipeline
{
    public interface IPipeChannel
    {
        Task<TResponse> Handle<TRequest, TResponse>(TRequest request);
        Task Handle<TMessage>(TMessage request);
    }
}
