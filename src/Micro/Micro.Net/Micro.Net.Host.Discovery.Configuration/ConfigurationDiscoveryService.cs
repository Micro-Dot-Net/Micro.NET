using System;
using System.Threading.Tasks;
using Micro.Net.Abstractions.Discovery;
using Microsoft.Extensions.Options;

namespace Micro.Net.Host.Discovery.Configuration
{
    public class ConfigurationDiscoveryService : DiscoveryService
    {
        private readonly IOptionsMonitor<ConfigurationDiscoveryOptions> _opts;

        public ConfigurationDiscoveryService(IOptionsMonitor<ConfigurationDiscoveryOptions> opts)
        {
            _opts = opts;
        }

        public Task Initialize()
        {
            return Task.CompletedTask;
        }

        public async Task Start()
        {
            _opts.OnChange(OnChange);
            OnChange(_opts.CurrentValue);
        }

        public Task Stop()
        {
            return Task.CompletedTask;
        }

        private ConfigurationDiscoveryOptions CurrentOptions = ConfigurationDiscoveryOptions.Default;
        private object _lock = new object();

        private void OnChange(ConfigurationDiscoveryOptions opts)
        {
            lock (_lock)
            {
                foreach (ServiceEntry service in opts.Services)
                {
                    
                }

                CurrentOptions = opts;
            }
        }

        public event ServiceEventDelegate ServiceDiscovered;
        public event ServiceEventDelegate ServiceLost;

        public void Enlist(Uri serviceAddress, string contractName, string assemblyName)
        {
            
        }

        public void Announce()
        {
            
        }

        public void Goodbye()
        {
            
        }
    }
}
