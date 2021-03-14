using System;

namespace Micro.Net.Abstractions
{
    public class InitializationException : ApplicationException
    {
        public InitializationException(string message, int code) : base(message)
        {
            HResult = code;
        }

        public static InitializationException FeatureNotSupported(string featureName) => new InitializationException($"Failure to initialize feature: {featureName}. Please check platform compatibility.", 600);

    }
}