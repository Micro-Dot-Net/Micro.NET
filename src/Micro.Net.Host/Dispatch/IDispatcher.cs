using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Micro.Net.Abstractions;

namespace Micro.Net.Dispatch
{
    public interface IDispatcher
    {
        ISet<DispatcherFeature> Features { get; }
        IEnumerable<(Type,Type)> Available { get; }
        Task Handle<TRequest,TResponse>(DispatchContext<TRequest,TResponse> messageContext) where TRequest : IContract<TResponse>;
    }
}