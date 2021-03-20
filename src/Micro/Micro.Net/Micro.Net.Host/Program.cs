using System;
using System.Reflection;
using Fclp;
using Micro.Net.Abstractions.Configuration;
using Micro.Net.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Micro.Net.Host
{
    class Program
    {
        private static MicroHostArguments hostArgs;

        static void Main(string[] args)
        {
            FluentCommandLineParser<MicroHostArguments> argParser = new FluentCommandLineParser<MicroHostArguments>();

            argParser
                .Setup(arg => arg.TargetAssembly)
                .As('t', "target")
                .Required();

            argParser
                .Setup(arg => arg.ConfigurationClass)
                .As('c', "config")
                .Required();

            argParser.Parse(args);

            hostArgs = argParser.Object;

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddHostedService<MicroHost>()
                        .AddSingleton<MicroHostArguments>(hostArgs);

                    IMicroserviceConfigurer configurer = (IMicroserviceConfigurer)Activator.CreateInstanceFrom(hostArgs.TargetAssembly, hostArgs.ConfigurationClass)?.Unwrap();

                    if (configurer == null)
                    {
                        throw new ArgumentException();
                    }

                    HostedMicroserviceConfigurable configurable = new HostedMicroserviceConfigurable();

                    configurer.ConfigureMicroservice(configurable);

                    configurable.ConfigureContainer(services, hostContext.Configuration);

                    services.Scan(scan => 
                        scan.FromApplicationDependencies()
                            
                        );
                });
    }
}
