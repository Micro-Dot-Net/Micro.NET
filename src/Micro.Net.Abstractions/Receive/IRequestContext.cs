using System.Collections.Generic;

namespace Micro.Net.Receive
{
    public interface IRequestContext<TRequest>
    {
        Dictionary<string, string[]> Headers { get; set; }
        TRequest Payload { get; set; }
    }
}