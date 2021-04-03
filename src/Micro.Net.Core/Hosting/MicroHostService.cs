using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Micro.Net.Core.Abstractions.Management;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Micro.Net.Core.Hosting
{
    internal class MicroHostService : BackgroundService
    {
        private readonly IEnumerable<IMicroComponent> _components;

        public MicroHostService(IEnumerable<IMicroComponent> components)
        {
            _components = components;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (IStartable startable in _components.OfType<IStartable>())
            {
                await startable.Start(cancellationToken);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (IStoppable stoppable in _components.OfType<IStoppable>())
            {
                await stoppable.Stop(cancellationToken);
            }
        }
    }
}
