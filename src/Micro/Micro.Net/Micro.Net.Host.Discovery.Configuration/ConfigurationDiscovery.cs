using Micro.Net.Abstractions.Discovery;

namespace Micro.Net.Host.Discovery.Configuration
{
    public class ConfigurationDiscovery : IDiscovery
    {
        private string _sectionName = string.Empty;

        public ConfigurationDiscovery FromSection(string name)
        {
            _sectionName = name;

            return this;
        }
    }
}