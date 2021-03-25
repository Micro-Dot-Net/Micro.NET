using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using MediatR;
using Micro.Net.Host.Dispatch;
using Micro.Net.Host.Dispatch.Http;

namespace Micro.Net
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IServiceProvider provider = null;

            IServiceCollection collection = new ServiceCollection();
            
            collection.AddTransient<IRequestHandler<ReceiveContext<TestRequest, TestResponse>,Unit>, HandlerShell<TestRequest, TestResponse>>();
            collection.AddTransient<IHandle<TestRequest, TestResponse>, TestHandler>();

            collection
                .AddTransient<IRequestHandler<DispatchContext<TestCommand, ValueTuple>, Unit>,
                    DispatchManager<TestCommand, ValueTuple>>();

            collection.AddTransient<IDispatcher, HttpDispatcher>();
            collection.AddTransient<HttpDispatcherConfiguration>(sc => new HttpDispatcherConfiguration()
            {
                Routes = new Dictionary<(Type, Type), (Uri, HttpMethod)>()
                {
                    { (typeof(TestCommand),typeof(ValueTuple)), (new Uri("http://localhost:8881/micro2/test"), HttpMethod.Post) }
                },
                DefaultHeaders = new Dictionary<string, string[]>()
                {
                    { "X-Token", new []{ "159e9497-553a-41ad-88c4-2af0f5faf7f2" } }
                }
            });

            collection.AddTransient<HttpClient>();

            ServiceFactory svcFactory = new ServiceFactory(type => provider.GetService(type));

            MediatR.IMediator mediator = new MediatR.Mediator(svcFactory);

            collection.AddSingleton<IMediator>(mediator);

            provider = collection.BuildServiceProvider(new ServiceProviderOptions());

            HttpReceiverConfiguration config = new HttpReceiverConfiguration()
            {
                BaseUris = new[] { "http://localhost:8880/" },
                PathMaps = new System.Collections.Generic.Dictionary<string, (Type, Type)>()
                {
                    { "/micro/test", (typeof(TestRequest), typeof(TestResponse)) }
                }
            };

            HttpReceiver receiver = new HttpReceiver(mediator, new System.Net.HttpListener(), config);

            receiver.Start(CancellationToken.None).GetAwaiter();

            Console.ReadLine();

            //CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });
    }
}
