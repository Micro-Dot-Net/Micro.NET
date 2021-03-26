using System;

namespace Micro.Net.Abstractions.Activities
{
    public interface IActivityContract<TLog> where TLog : IActivityLog
    {
        Guid CorrelationId { get; set; }
    }
}