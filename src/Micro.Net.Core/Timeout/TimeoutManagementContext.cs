using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Timeout;
using Micro.Net.Receive;
using NodaTime;
using OneOf;

namespace Micro.Net.Core.Timeout
{
    public class TimeoutContext<TTimeout> : ContextBase, ITimeoutContext<TTimeout> where TTimeout : ITimeout
    {
        public OneOf<Instant, Duration> Delay { get; set; }
        public IRequestContext<TTimeout> Request { get; set; }
        public TimeoutOptions Options { get; set; }
    }
}
