namespace Micro.Net.Exceptions
{
    public class MicroHostException : MicroException
    {
        public MicroHostException() : this("An unspecified error with a Hosting component has occurred!", -1)
        {

        }

        public MicroHostException(string message, int errorCode) : base(message, errorCode)
        {

        }
    }
}