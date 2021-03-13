namespace Micro.Net.Abstractions.Configuration
{
    public interface IMicroserviceConfigurer
    {
        void ConfigureMicroservice(IMicroserviceConfigurable config);
    }
}
