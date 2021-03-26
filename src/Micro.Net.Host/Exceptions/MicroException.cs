using System;

namespace Micro.Net.Exceptions
{
    /// <summary>
    /// Base class for all framework exceptions
    /// </summary>
    public class MicroException : ApplicationException
    {
        public int ErrorCode { get; }

        public MicroException()
        {

        }

        public MicroException(string message, int code) : base(message)
        {
            ErrorCode = code;
        }
    }
}