using System;

namespace Micro.Net.Abstractions.Messages.Dispatch
{
    public class DispatchException : ApplicationException
    {
        public DispatchException(string message, int resultCode) : base(message)
        {
            HResult = resultCode;
            
        }

        public static DispatchException ConnectionFail => new DispatchException("Dispatcher failed to connect during a synchronous dispatch attempt.", 500);
        public static DispatchException UnexpectedResponse => new DispatchException("Dispatcher received an unexpected response from the remote endpoint.", 501);
        public static DispatchException ConfigurationRelatedError => new DispatchException("Dispatcher encountered an error likely related to configuration.", 502);
    }
}