using System;

namespace Micro.Net.Host.Abstractions.Activities
{
    public interface IActivityContract<TLog> where TLog : IActivityLog
    {
        Guid CorrelationId { get; set; }
    }
}