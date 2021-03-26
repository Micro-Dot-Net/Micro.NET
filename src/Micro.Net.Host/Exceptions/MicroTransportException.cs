namespace Micro.Net.Exceptions
{
    public class MicroTransportException : MicroException
    {
        public MicroTransportException() : base("An unspecified error with a Transport component has occurred.", -1)
        {

        }

        public MicroTransportException(string message, int errorCode) : base(message, errorCode)
        {

        }
    }
}