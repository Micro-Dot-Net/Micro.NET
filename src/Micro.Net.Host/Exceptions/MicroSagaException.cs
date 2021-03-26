namespace Micro.Net.Exceptions
{
    public class MicroSagaException : MicroHostException
    {
        public MicroSagaException() : this("An unspecified error with a Saga component has occurred!", -1)
        {

        }

        public MicroSagaException(string message, int errorCode) : base(message, errorCode)
        {

        }
    }
}