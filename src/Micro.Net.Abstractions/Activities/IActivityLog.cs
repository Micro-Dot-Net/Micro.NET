using System;

namespace Micro.Net.Abstractions.Activities
{
    public interface IActivityLog
    {
        Guid CorrelationId { get; set; }
    }
}