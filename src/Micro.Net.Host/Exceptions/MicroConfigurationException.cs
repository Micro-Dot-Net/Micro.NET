using System.Collections.Generic;
using System.Linq;

namespace Micro.Net.Exceptions
{
    public class MicroConfigurationException : MicroException
    {
        public MicroConfigurationException() : base("An unspecified configuration-related error has occurred.", -1)
        {

        }

        public MicroConfigurationException(string message, int errorCode) : base(message, errorCode)
        {

        }

        public static MicroConfigurationException MissingRegistrations(Dictionary<string, string> hints) => 
            new MicroConfigurationException($"One or more component(s) were not able to be resolved, check dependency config. Missing components: {string.Join("; ", hints.Select(x => $"{x.Key} [{x.Value}]"))}.", 100);
    }
}