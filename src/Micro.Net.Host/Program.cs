using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Micro.Net.Abstractions;
using Micro.Net.Core.Configuration;
using Micro.Net.Core.Pipeline;
using Micro.Net.Dispatch;
using Micro.Net.Handling;
using Micro.Net.Receive;
using Micro.Net.Storage.FileSystem;
using Micro.Net.Test;
using Newtonsoft.Json;
using JsonSerializer = Micro.Net.Serializing.JsonSerializer;
using Micro.Net.Transport.Http;
using Micro.Net.Transport.Feather;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Micro.Net
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging(conf =>
                    {
                        conf.AddConsole(opts =>
                        {
                            opts.LogToStandardErrorThreshold = LogLevel.Trace;
                        });
                    });
                    services.UseMicroNet(cfg =>
                    {
                        cfg
                            .UseFeatherHttpReceiver(config =>
                            {
                                config.BaseUris.Add("http://0.0.0.0:8881");
                                config.BaseUris.Add("http://::8882");
                                config.PathMaps.Add(("/micro/test", HttpMethod.Post), (typeof(TestRequest), typeof(TestResponse)));
                            })
                            //.UseHttpReceiver(config =>
                            //{
                            //    config.BaseUris.Add("http://+:8881/");
                            //    config.PathMaps.Add("/micro/test", (typeof(TestRequest), typeof(TestResponse)));
                            //})
                            .UseHttpDispatcher(config =>
                            {
                                config.Routes.Add((typeof(TestCommand), typeof(ValueTuple)), (new Uri("http://localhost:8882/micro2/test"), HttpMethod.Post));
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
