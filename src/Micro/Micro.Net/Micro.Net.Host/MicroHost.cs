using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Micro.Net.Abstractions.Components;
using Micro.Net.Abstractions.Lifecycle;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Micro.Net.Host
{
    public class MicroHost : BackgroundWorker, IHostedService
    {
        private readonly IServiceProvider _provider;
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public MicroHost(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            IEnumerable<IRun<LifeCycleStep.PlatformInitialize>> initRunners = _provider.GetService<IEnumerable<IRun<LifeCycleStep.PlatformInitialize>>>();

            if (initRunners != null && initRunners.Any())
            {
                RunContext<LifeCycleStep.PlatformInitialize> ctx = new RunContext<LifeCycleStep.PlatformInitialize>();

                await Task.WhenAll(initRunners.Select(x => x.Run(ctx, cancellationToken)));
            }

            IServiceCollection collection = await step_ContainerInit();

            await step_ComponentsInit(collection.BuildServiceProvider());

            step_Run(_tokenSource.Token);
        }

        private async Task<IServiceCollection> step_ContainerInit()
        {
            IServiceCollection collection = new ServiceCollection();

            await subStep_ScannerInit(collection);

            await subStep_ProduceConfig(collection);

            return collection;
        }

        private async Task subStep_ScannerInit(IServiceCollection collection)
        {
            
        }

        private async Task subStep_ProduceConfig(IServiceCollection collection)
        {
            
        }

        private async Task step_ComponentsInit(IServiceProvider provider)
        {
            IEnumerable<IRun<LifeCycleStep.PlatformRun>> runners =
                provider.GetServices<IRun<LifeCycleStep.PlatformRun>>();

            RunContext<LifeCycleStep.PlatformRun> context = new RunContext<LifeCycleStep.PlatformRun>();

            await Task.WhenAll(runners.Select(x => x.Run(context, _tokenSource.Token)));

            await subStep_StorageCompInit(provider);
            await subStep_CachingCompInit(provider);
            await subStep_DiscoveryCompInit(provider);
            await subStep_TransportCompInit(provider);
        }

        private async Task subStep_StorageCompInit(IServiceProvider provider)
        {
            IEnumerable<IComponent<ComponentKind.Storage>> components =
                provider.GetServices<IComponent<ComponentKind.Storage>>();

            await Task.WhenAll(components.Select(x => x.Initialize(_tokenSource.Token)));
        }

        private async Task subStep_CachingCompInit(IServiceProvider provider)
        {
            IEnumerable<IComponent<ComponentKind.Cache>> components =
                provider.GetServices<IComponent<ComponentKind.Cache>>();

            await Task.WhenAll(components.Select(x => x.Initialize(_tokenSource.Token)));
        }

        private async Task subStep_DiscoveryCompInit(IServiceProvider provider)
        {
            IEnumerable<IComponent<ComponentKind.Discovery>> components =
                provider.GetServices<IComponent<ComponentKind.Discovery>>();

            await Task.WhenAll(components.Select(x => x.Initialize(_tokenSource.Token)));
        }

        private async Task subStep_TransportCompInit(IServiceProvider provider)
        {
            IEnumerable<IComponent<ComponentKind.Transport.Dispatch>> dispatchComponents =
                provider.GetServices<IComponent<ComponentKind.Transport.Dispatch>>();
            IEnumerable<IComponent<ComponentKind.Transport>> generalComponents =
                provider.GetServices<IComponent<ComponentKind.Transport>>();
            IEnumerable<IComponent<ComponentKind.Transport.Receive>> receiveComponents =
                provider.GetServices<IComponent<ComponentKind.Transport.Receive>>();

            await Task.WhenAll(dispatchComponents.Select(x => x.Initialize(_tokenSource.Token)));
            await Task.WhenAll(generalComponents.Select(x => x.Initialize(_tokenSource.Token)));
            await Task.WhenAll(receiveComponents.Select(x => x.Initialize(_tokenSource.Token)));
        }

        private async Task step_Run(CancellationToken cancellationToken)
        {
            IEnumerable<IRun<LifeCycleStep.PlatformRun>> runners = _provider.GetService<IEnumerable<IRun<LifeCycleStep.PlatformRun>>>();

            if (runners != null && runners.Any())
            {
                RunContext<LifeCycleStep.PlatformRun> ctx = new RunContext<LifeCycleStep.PlatformRun>();

                await Task.WhenAll(runners.Select(x => x.Run(ctx, _tokenSource.Token)));
            }

            await step_ComponentsStart(_provider);
        }
        
        private async Task step_ComponentsStart(IServiceProvider provider)
        {
            await subStep_StorageCompStart(provider);
            await subStep_CachingCompStart(provider);
            await subStep_DiscoveryCompStart(provider);
            await subStep_TransportCompStart(provider);
        }

        private async Task subStep_StorageCompStart(IServiceProvider provider)
        {
            IEnumerable<IComponent<ComponentKind.Storage>> components =
                provider.GetServices<IComponent<ComponentKind.Storage>>();

            await Task.WhenAll(components.Select(x => x.Start(_tokenSource.Token)));
        }

        private async Task subStep_CachingCompStart(IServiceProvider provider)
        {
            IEnumerable<IComponent<ComponentKind.Cache>> components =
                provider.GetServices<IComponent<ComponentKind.Cache>>();

            await Task.WhenAll(components.Select(x => x.Start(_tokenSource.Token)));
        }

        private async Task subStep_DiscoveryCompStart(IServiceProvider provider)
        {
            IEnumerable<IComponent<ComponentKind.Discovery>> components =
                provider.GetServices<IComponent<ComponentKind.Discovery>>();

            await Task.WhenAll(components.Select(x => x.Start(_tokenSource.Token)));
        }

        private async Task subStep_TransportCompStart(IServiceProvider provider)
        {
            IEnumerable<IComponent<ComponentKind.Transport.Dispatch>> dispatchComponents =
                provider.GetServices<IComponent<ComponentKind.Transport.Dispatch>>();
            IEnumerable<IComponent<ComponentKind.Transport>> generalComponents =
                provider.GetServices<IComponent<ComponentKind.Transport>>();
            IEnumerable<IComponent<ComponentKind.Transport.Receive>> receiveComponents =
                provider.GetServices<IComponent<ComponentKind.Transport.Receive>>();

            await Task.WhenAll(dispatchComponents.Select(x => x.Start(_tokenSource.Token)));
            await Task.WhenAll(generalComponents.Select(x => x.Start(_tokenSource.Token)));
            await Task.WhenAll(receiveComponents.Select(x => x.Start(_tokenSource.Token)));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            
            
            IEnumerable<IRun<LifeCycleStep.PlatformShutdown>> runners = _provider.GetService<IEnumerable<IRun<LifeCycleStep.PlatformShutdown>>>();

            if (runners != null && runners.Any())
            {
                RunContext<LifeCycleStep.PlatformShutdown> ctx = new RunContext<LifeCycleStep.PlatformShutdown>();

                await Task.WhenAll(runners.Select(x => x.Run(ctx, _tokenSource.Token)).ToArray());
            }
            
            _tokenSource.Cancel();
        }

    }
}