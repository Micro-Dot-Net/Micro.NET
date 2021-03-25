using System;

namespace Micro.Net.Host.Abstractions.Activities
{
    public interface IActivityLog
    {
        Guid CorrelationId { get; set; }
    }
}