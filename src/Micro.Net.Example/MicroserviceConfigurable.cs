using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using Micro.Net.Abstractions.Hosting;
using Micro.Net.Core.Configuration;
using Micro.Net.Core.Pipeline;
using Micro.Net.Storage.FileSystem;
using Micro.Net.Test;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Micro.Net.Transport.Http;
using Microsoft.Extensions.Configuration;
using JsonSerializer = Micro.Net.Serializing.JsonSerializer;

namespace Micro.Net.Example
{
    public class MicroserviceConfigurable : IMicroserviceConfigurable
    {
        public void Configure(IMicroConfigurer cfg, IConfiguration configuration)
        {


            cfg
                    //.UseFeatherHttpReceiver(config =>
                    //{
                    //    config.BaseUris.Add("http://0.0.0.0:8881");
                    //    config.BaseUris.Add("http://::8882");
                    //    config.PathMaps.Add(("/micro/test", HttpMethod.Post), (typeof(TestRequest), typeof(TestResponse)));
                    //})
                    .UseHttpReceiver(config =>
                    {
                        config.BaseUris.Add("http://+:8881/");
                        config.PathMaps.Add("/micro/test", (typeof(TestRequest), typeof(TestResponse)));
                    })
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
             
        }
        
    }
}
