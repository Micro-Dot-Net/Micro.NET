using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public Task Initialize(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            _opts.OnChange(OnChange);
            OnChange(_opts.CurrentValue);
        }

        public Task Stop(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private ConfigurationDiscoveryOptions CurrentOptions = ConfigurationDiscoveryOptions.Default;
        private object _lock = new object();

        private void OnChange(ConfigurationDiscoveryOptions opts)
        {
            lock (_lock)
            {
                IEnumerable<ServiceEntry> discovered = opts.Services.Except(CurrentOptions.Services, ServiceEntryEqualityComparer.Default);
                IEnumerable<ServiceEntry> lost = CurrentOptions.Services.Except(opts.Services, ServiceEntryEqualityComparer.Default);

                foreach (ServiceEntry service in discovered)
                {
                    ServiceDiscovered?.Invoke(service.Address, service.Contract, service.Assembly);
                }

                foreach (ServiceEntry service in lost)
                {
                    ServiceLost?.Invoke(service.Address, service.Contract, service.Assembly);
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
