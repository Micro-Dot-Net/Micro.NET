namespace Micro.Net
{
    public class MicroDispatcherException : MicroTransportException
    {
        public static MicroReceiverException NoMapping => new MicroReceiverException() { HResult = 404 };
    }
}