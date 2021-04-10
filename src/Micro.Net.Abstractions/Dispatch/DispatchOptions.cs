using System;
using System.Collections.Generic;

namespace Micro.Net.Dispatch
{
    public class DispatchOptions
    {
        internal DispatchOptions(){}
        public bool ThrowOnFailure { get; set; }
        internal ICollection<(string, string)> Headers { get; private set; }
        public DispatchMode DispatchMode { get; set; }
        public DispatchFlags DispatchFlags { get; set; }


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

    public enum DispatchMode
    {
        None,
        PreferAsync,
        PreferSync,
        PreferOutbox,
        RequireAsync,
        RequireSync,
        RequireOutbox
    }

    [Flags]
    public enum DispatchFlags
    {
        FireAndForget = 1,
        PrecheckOutbox = 2
    }
}