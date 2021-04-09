using System;
using System.Numerics;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Micro.Net.Core.Pipeline
{
    public class LoggingPipeStepFactory : IPipelineStepFactory
    {
        public BigInteger Priority { get; } = 0;

        public async Task<IPipelineStep<TRequest, TResponse>> Create<TRequest, TResponse>()
        {
            return new GenericPipeStep<TRequest, TResponse>(next => async req =>
            {
                Console.WriteLine(JsonConvert.SerializeObject(req, Formatting.Indented));

                return await next.Invoke(req);
            });
        }
    }
}