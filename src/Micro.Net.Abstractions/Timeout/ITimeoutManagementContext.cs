using System;
using System.Collections.Generic;
using System.Text;
using Micro.Net.Receive;
using NodaTime;
using OneOf;

namespace Micro.Net.Abstractions.Timeout
{
    public interface ITimeoutContext<TTimeout> : IContextBase where TTimeout : ITimeout
    {
        OneOf<Instant,Duration> Delay { get; set; }
        IRequestContext<TTimeout> Request { get; set; }
        TimeoutOptions Options { get; set; }
    }

    public interface ITimeout : IContract
    {

    }

    public class TimeoutOptions
    {

    }
}
