using System;
using System.Threading.Tasks;
using Micro.Net.Abstractions.Processing;

namespace Micro.Net.Processing.Piping
{
    public class DuplexPipeline<T1, T2> : IDuplexPipeline<T1, T2>
    {
        private readonly Pipeline<T1> _firstPipe;
        private readonly Pipeline<T2> _secondPipe;

        public DuplexPipeline(Pipeline<T1> firstPipe, Pipeline<T2> secondPipe)
        {
            _firstPipe = firstPipe;
            _secondPipe = secondPipe;
        }
        
        public async Task Execute(T1 context)
        {
            await _firstPipe.Execute(context);
        }

        public async Task Step(T2 context, PipelineDelegate<T2> next)
        {
            throw new NotImplementedException();
        }
    }
}