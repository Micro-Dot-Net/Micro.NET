using System.Threading;
using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Micro.Net.Core.Abstractions.Pipeline;
using Micro.Net.Transport.Generic;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Connections;

namespace Micro.Net.Transport.KestrelHttp
{
    public class KestrelReceiver : GenericReceiverBase
    {
        private readonly KestrelReceiverConfiguration _config;
        private readonly IContextFactory _contextFactory;

        public KestrelReceiver(IPipeChannel pipeChannel, KestrelReceiverConfiguration config, IContextFactory contextFactory) : base(pipeChannel)
        {
            _config = config;
            _contextFactory = contextFactory;
        }

        public override async Task Start(CancellationToken cancellationToken)
        {

        }

        public override async Task Stop(CancellationToken cancellationToken)
        {

        }
    }
}
