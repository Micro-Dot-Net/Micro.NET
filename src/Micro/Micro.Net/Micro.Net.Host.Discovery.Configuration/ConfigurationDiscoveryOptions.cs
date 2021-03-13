namespace Micro.Net.Host.Discovery.Configuration
{
    public class ConfigurationDiscoveryOptions
    {
        public ServiceEntry[] Services { get; set; }

        public static ConfigurationDiscoveryOptions Default => new ConfigurationDiscoveryOptions(){Services = new ServiceEntry[0]};
    }
}