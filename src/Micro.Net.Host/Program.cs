using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Hosting;
using Micro.Net.Core.Configuration;
using Micro.Net.Core.Pipeline;
using Micro.Net.Dispatch;
using Micro.Net.Exceptions;
using Micro.Net.Handling;
using Micro.Net.Receive;
using Micro.Net.Storage.FileSystem;
using Newtonsoft.Json;
using JsonSerializer = Micro.Net.Serializing.JsonSerializer;
using Micro.Net.Transport.Http;
using Micro.Net.Transport.Feather;
using Microsoft.Extensions.Configuration;
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
                    MicroHostConfiguration hostConfig = hostContext.Configuration.GetSection("Host").Get<MicroHostConfiguration>();

                    Assembly assembly = Assembly.LoadFrom(hostConfig.Assembly);

                    IEnumerable<Type> types = assembly.GetExportedTypes().Where(t => t.IsAssignableTo(typeof(IMicroserviceConfigurable)));

                    IMicroserviceConfigurable cfgbl = default;

                    if (!types.Any())
                    {
                        throw new MicroHostException("No configurable types found!", 1401);
                    }

                    if (string.IsNullOrWhiteSpace(hostConfig.ConfigClass))
                    {
                        if (types.Count() > 1)
                        {
                            throw new MicroHostException("More than one configurable type found, but none were specified! Designate a configurable type as 'Host:ConfigClass'.", 1402);
                        }
                        else
                        {
                            cfgbl = (IMicroserviceConfigurable)Activator.CreateInstance(types.Single());
                        }
                    }
                    else
                    {
                        types = types.Where(t =>
                            t.Name.Equals(hostConfig.ConfigClass) || t.FullName.Equals(hostConfig.ConfigClass) ||
                            t.AssemblyQualifiedName.Equals(hostConfig.ConfigClass));

                        if (types.Count() > 1)
                        {
                            throw new MicroHostException("Multiple configurable types found matching designated configurable! Use FullName or AssemblyQualifiedName instead.", 1403);
                        }
                        else if (!types.Any())
                        {
                            throw new MicroHostException($"No configurable types found with name '{hostConfig.ConfigClass}'!", 1404);
                        }
                        else
                        {
                            cfgbl = (IMicroserviceConfigurable)Activator.CreateInstance(types.Single());
                        }
                    }

                    services.UseMicroNet(mcfg => cfgbl.Configure(mcfg, hostContext.Configuration));

                    if (services.All(s => s.ServiceType != typeof(ILoggerFactory)))
                    {
                        services.AddLogging(conf =>
                        {
                            conf.AddConsole();
                        });
                    }
                });
    }
}
