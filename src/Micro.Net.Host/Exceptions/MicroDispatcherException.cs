using System;

namespace Micro.Net.Exceptions
{
    public class MicroDispatcherException : MicroTransportException
    {
        public MicroDispatcherException() : this("An unspecified error with a Dispatcher component has occurred!", -1)
        {

        }

        public MicroDispatcherException(string message, int errorCode) : base(message, errorCode)
        {

        }

        public static MicroDispatcherException NoMapping(Type reqType, Type respType) => new MicroDispatcherException($"No dispatcher was found with an available mapping for <{(respType == typeof(ValueTuple) ? reqType.FullName : $"{reqType.FullName}, {respType.FullName}" )}>! This is likely a configuration issue.", 404);
    }
}