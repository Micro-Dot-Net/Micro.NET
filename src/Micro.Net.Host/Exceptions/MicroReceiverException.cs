namespace Micro.Net
{
    public class MicroReceiverException : MicroTransportException
    {
        public static MicroReceiverException NoMapping => new MicroReceiverException(){ HResult = 404 };
    }
}