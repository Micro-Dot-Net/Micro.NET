using System;
using System.Threading.Tasks;
using Micro.Net.Exceptions;

namespace Micro.Net.Core.Pipeline
{
    public class GenericPipeTail<TRequest, TResponse> : IPipelineTail<TRequest,TResponse>
    {
        private readonly Func<TRequest, Task<TResponse>> _endware;

        public GenericPipeTail(Func<TRequest,Task<TResponse>> endware)
        {
            _endware = endware ?? throw new MicroException(/* TODO: Define a better exception */);
        }

        public async Task<TResponse> Handle(TRequest request)
        {
            return await _endware.Invoke(request);
        }
    }
}