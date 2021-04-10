using System.Collections.Generic;

namespace Micro.Net.Transport.FileSystem
{
    public class Envelope<TValue>
    {
        public Dictionary<string, string[]> Headers { get; set; }
        public TValue Request { get; set; }
    }
}