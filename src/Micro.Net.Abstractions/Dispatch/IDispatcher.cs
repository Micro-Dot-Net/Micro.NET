using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Micro.Net.Core.Abstractions.Management;

namespace Micro.Net.Dispatch
{
    public interface IDispatcher : IMicroComponent
    {
        ISet<DispatcherFeature> Features { get; }
        IEnumerable<(Type,Type)> Available { get; }
        Task Handle<TRequest,TResponse>(IDispatchContext<TRequest,TResponse> messageContext) where TRequest : IContract<TResponse>;
    }
}