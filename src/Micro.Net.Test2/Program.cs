using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Micro.Net.Handling;
using Micro.Net.Receive;
using Micro.Net.Receive.Http;
using Micro.Net.Test;
using Microsoft.Extensions.Logging;

namespace Micro.Net.Test2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IServiceProvider provider = null;

            IServiceCollection collection = new ServiceCollection();

            collection.AddTransient<IRequestHandler<ReceiveContext<TestCommand, ValueTuple>, Unit>, ReceiveHandlerShell<TestCommand, ValueTuple>>();
            collection.AddTransient<IHandle<TestCommand>, TestCommandHandler>();

            //collection.AddTransient<IDispatcher, HttpDispatcher>();
            //collection.AddTransient<HttpDispatcherConfiguration>(sc => new HttpDispatcherConfiguration()
            //{
            //    Routes =
            //    {
            //        { (typeof(TestCommand),typeof(ValueTuple)), (new Uri("http://localhost:8881/micro2/test"), HttpMethod.Post) }
            //    },
            //    DefaultHeaders =
            //    {
            //        { "X-Token", new []{ "159e9497-553a-41ad-88c4-2af0f5faf7f2" } }
            //    }
            //});

            ServiceFactory svcFactory = new ServiceFactory(type => provider.GetService(type));

            MediatR.IMediator mediator = new MediatR.Mediator(svcFactory);

            collection.AddSingleton<IMediator>(mediator);

            provider = collection.BuildServiceProvider(new ServiceProviderOptions());

            HttpReceiverConfiguration config = new HttpReceiverConfiguration()
            {
                BaseUris = new[] { "http://localhost:8881/" },
                PathMaps = new System.Collections.Generic.Dictionary<string, (Type, Type)>()
                {
                    { "/micro2/test", (typeof(TestCommand), typeof(ValueTuple)) }
                }
            };

            HttpReceiver receiver = new HttpReceiver(mediator, new System.Net.HttpListener(), config);

            receiver.Start(CancellationToken.None).GetAwaiter();

            Console.ReadLine();
            //CreateHostBuilder(args).Build().Run();
        }

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureServices((hostContext, services) =>
        //        {
        //            services.AddHostedService<Worker>();
        //        });
    }

    public class TestCommandHandler : IHandle<TestCommand>
    {
        public TestCommandHandler()
        {

        }

        public async Task Handle(TestCommand message, HandlerContext context)
        {
            Console.WriteLine($"TestCommand Received! {message.Id}");
        }
    }
}
