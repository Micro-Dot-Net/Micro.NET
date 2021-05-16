using System;
using System.Threading;
using System.Threading.Tasks;
using Micro.Net.Core.Abstractions.Pipeline;
using Micro.Net.Transport.Generic;

namespace Micro.Net.Transport.Quartz
{
    public class QuartzScheduledReceiver : GenericReceiverBase
    {
        public QuartzScheduledReceiver(IPipeChannel pipeChannel) : base(pipeChannel)
        {
        }

        public override Task Start(CancellationToken cancellationToken)
        {
            return base.Start(cancellationToken);
        }

        public override Task Stop(CancellationToken cancellationToken)
        {
            return base.Stop(cancellationToken);
        }
    }

    public class QuartzScheduledReceiverConfiguration
    {

    }
}
