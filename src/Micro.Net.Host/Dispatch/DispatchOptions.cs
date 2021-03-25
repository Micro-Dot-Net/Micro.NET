using System.Collections.Generic;

namespace Micro.Net.Host.Dispatch
{
    public class DispatchOptions
    {
        public bool ThrowOnFailure { get; internal set; }
        internal ICollection<(string, string)> Headers { get; private set; }

        public void AddHeader(string key, string value)
        {
            Headers.Add((key, value));
        }

        public static DispatchOptions Create()
        {
            DispatchOptions opts = new DispatchOptions()
            {
                ThrowOnFailure = true,
                Headers = new List<(string, string)>()
            };

            return opts;
        }
    }
}