using System;
using Micro.Net.Exceptions;

namespace Micro.Net.Dispatch.Http
{
    public class HttpDispatcherException : MicroDispatcherException
    {
        public static HttpDispatcherException ConfigurationRelatedError => new HttpDispatcherException();
        public static HttpDispatcherException ConnectionFail => new HttpDispatcherException();

        public static HttpDispatcherException NoRouteFound(Type request, Type response) => new HttpDispatcherException() {HResult = 500, Data = {{ "RequestType", request }, {"ResponseType", response}}};
    }
}