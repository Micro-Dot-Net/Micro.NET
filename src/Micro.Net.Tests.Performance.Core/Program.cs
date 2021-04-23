using System;
using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Micro.Net.Core.Abstractions.Pipeline;
using Micro.Net.Core.Configuration;
using Micro.Net.Handling;
using Micro.Net.Receive;
using Microsoft.Extensions.DependencyInjection;
using NBench;

namespace Micro.Net.Tests.Performance.Core
{
    class Program
    {
        static int Main(string[] args)
        {
            return NBenchRunner.Run<Program>();
        }
    }

    public class CorePerformanceTests
    {
        private IServiceProvider _serviceProvider;
        private IPipeChannel _pipeChannel;

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.UseMicroNet(cfg =>
            {
                cfg.AddHandler<BenchmarkHandler, BenchmarkCommand>();
                cfg.AddReceivable<BenchmarkCommand, ValueTuple>();
            });

            _serviceProvider = serviceCollection.BuildServiceProvider();

            _pipeChannel = _serviceProvider.GetService<IPipeChannel>();
        }

        [PerfBenchmark(NumberOfIterations = 10, RunMode = RunMode.Iterations, TestMode = TestMode.Measurement)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        public void CorePipelineRoundTrip()
        {
            BenchmarkCommand command = new BenchmarkCommand();

            IReceiveContext<BenchmarkCommand, ValueTuple> commandContext =
                ReceiveContext.Create<BenchmarkCommand, ValueTuple>();

            commandContext.Request.Payload = command;

            _pipeChannel.Handle(commandContext);
        }
    }

    public class BenchmarkCommand : IContract { }

    public class BenchmarkHandler : IHandle<BenchmarkCommand>
    {
        public async Task Handle(BenchmarkCommand message, IHandlerContext context)
        {
            await Task.Delay(0);
        }
    }
}
