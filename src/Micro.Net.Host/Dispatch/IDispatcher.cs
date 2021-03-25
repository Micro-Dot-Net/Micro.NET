using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Micro.Net.Host.Dispatch
{
    public interface IDispatcher
    {
        IEnumerable<(Type,Type)> Available { get; }
        Task<TResponse> Handle<TRequest,TResponse>(TRequest message, DispatchOptions options);
    }
}