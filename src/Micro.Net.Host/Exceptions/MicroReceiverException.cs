namespace Micro.Net.Exceptions
{
    public class MicroReceiverException : MicroTransportException
    {
        public MicroReceiverException() : this("An unspecified error with a Receiver Transport component has occurred.", -1)
        {

        }

        public MicroReceiverException(string message, int errorCode) : base(message, errorCode)
        {

        }

        public static MicroReceiverException NoMapping => new MicroReceiverException("An unmapped message was received by this transport! This is likely a configuration issue.", 404);
    }
}