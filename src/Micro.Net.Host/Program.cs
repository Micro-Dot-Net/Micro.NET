using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using MediatR;
using Micro.Net.Abstractions;
using Micro.Net.Core.Configuration;
using Micro.Net.Core.Pipeline;
using Micro.Net.Dispatch;
using Micro.Net.Handling;
using Micro.Net.Receive;
using Micro.Net.Storage.FileSystem;
using Micro.Net.Test;
using Micro.Net.Transport.Http;
using Newtonsoft.Json;
using JsonSerializer = Micro.Net.Serializing.JsonSerializer;

namespace Micro.Net
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //IServiceProvider provider = null;

            //IServiceCollection collection = new ServiceCollection();
            
            //collection.AddTransient<IRequestHandler<ReceiveContext<TestRequest, TestResponse>,Unit>, HandlerShell<TestRequest, TestResponse>>();
            //collection.AddTransient<IHandle<TestRequest, TestResponse>, TestHandler>();

            //collection
            //    .AddTransient<IRequestHandler<DispatchManagementContext<TestCommand, ValueTuple>, Unit>,
            //        DispatchManager<TestCommand, ValueTuple>>();

            //collection.AddTransient<IDispatcher, HttpDispatcher>();
            //collection.AddTransient<HttpDispatcherConfiguration>(sc => new HttpDispatcherConfiguration()
            //{
            //    Routes = new Dictionary<(Type, Type), (Uri, HttpMethod)>()
            //    {
            //        { (typeof(TestCommand),typeof(ValueTuple)), (new Uri("http://localhost:8881/micro2/test"), HttpMethod.Post) }
            //    },
            //    DefaultHeaders = new Dictionary<string, string[]>()
            //    {
            //        { "X-Token", new []{ "159e9497-553a-41ad-88c4-2af0f5faf7f2" } }
            //    }
            //});

            //collection.AddTransient<HttpClient>();

            //collection.AddSingleton(new HandlerConfiguration()
            //{
            //    Dispatch_ThrowOnFault = true
            //});

            //ServiceFactory svcFactory = new ServiceFactory(type => provider.GetService(type));

            //MediatR.IMediator mediator = new MediatR.Mediator(svcFactory);

            //collection.AddSingleton<IMediator>(mediator);

            //provider = collection.BuildServiceProvider(new ServiceProviderOptions());

            //HttpReceiverConfiguration config = new HttpReceiverConfiguration()
            //{
            //    BaseUris = new[] { "http://localhost:8880/" },
            //    PathMaps = new System.Collections.Generic.Dictionary<string, (Type, Type)>()
            //    {
            //        { "/micro/test", (typeof(TestRequest), typeof(TestResponse)) }
            //    }
            //};

            //HttpReceiver receiver = new HttpReceiver(mediator, new System.Net.HttpListener(), config);

            //receiver.Start(CancellationToken.None).GetAwaiter();

            //Console.ReadLine();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.UseMicroNet(cfg =>
                    {
                        cfg
                            .UseHttpReceiver(config =>
                            {
                                config.BaseUris.Add("http://localhost:8880/");
                                config.PathMaps.Add("/micro/test", (typeof(TestRequest), typeof(TestResponse)));
                            })
                            .UseHttpDispatcher(config =>
                            {
                                config.Routes.Add((typeof(TestCommand), typeof(ValueTuple)), (new Uri("http://localhost:8881/micro2/test"), HttpMethod.Post));
                                config.DefaultHeaders.Add("X-Token", new[] { "159e9497-553a-41ad-88c4-2af0f5faf7f2" });
                            })
                            .AddHandler<TestHandler, TestRequest, TestResponse>()
                            .AddSerializer<JsonSerializer>()
                            .AddComponent<JsonSerializerSettings>(ServiceLifetime.Singleton)
                            .AddComponent<IPipelineStepFactory, LoggingPipeStepFactory>(ServiceLifetime.Singleton)
                            .UseFileSagaPersistence(config =>
                            {
                                config.SetDefaults(def =>
                                {
                                    def.NamePattern = "{DataType}_{Key}.saga";
                                    def.StoragePath = Path.Combine(AppContext.BaseDirectory, "Sagas");
                                    def.KeepProcessed = false;
                                });
                            })
                            .ConfigureSystem(config =>
                            {
                                config.Dispatch.ThrowOnFault = false;
                            });
                    });
                });
    }
}
