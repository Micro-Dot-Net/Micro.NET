using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Micro.Net.Dispatch
{
    public interface IDispatcher
    {
        ISet<DispatcherFeature> Features { get; }
        IEnumerable<(Type,Type)> Available { get; }
        Task<TResponse> Handle<TRequest,TResponse>(TRequest message, DispatchOptions options);
    }
}