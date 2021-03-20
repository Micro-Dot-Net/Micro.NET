namespace Micro.Net.Abstractions.Exceptions
{
    public class MicroHostException : MicroException
    {

    }

    public class MicroTransportException : MicroException
    {
        
    }

    public class MicroReceiverException : MicroTransportException
    {
        public static MicroReceiverException NoMapping => new MicroReceiverException();
    }

    public class MicroDispatcherException : MicroTransportException
    {
        
    }
}