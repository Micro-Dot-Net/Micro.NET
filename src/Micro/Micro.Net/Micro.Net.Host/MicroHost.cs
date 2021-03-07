using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Micro.Net.Host
{
    public class MicroHost : BackgroundWorker, IHostedService
    {


        public MicroHost(MicroHostArguments args)
        {

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}