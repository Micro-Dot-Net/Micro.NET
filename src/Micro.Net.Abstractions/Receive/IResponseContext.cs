using System.Collections.Generic;

namespace Micro.Net.Receive
{
    public interface IResponseContext<TResponse>
    {
        Dictionary<string, string[]> Headers { get; set; }
        TResponse Payload { get; set; }
    }
}