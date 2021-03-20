namespace Micro.Net.Abstractions.Exceptions
{
    public class MicroConfigurationException : MicroException
    {
        public static MicroConfigurationException AlreadyConfigured(string component) => new MicroConfigurationException();
    }
}