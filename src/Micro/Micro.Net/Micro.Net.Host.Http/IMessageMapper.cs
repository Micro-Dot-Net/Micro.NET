using System;
using Microsoft.AspNetCore.Http;

namespace Micro.Net.Host.Http
{
    public interface IMessageMapper
    {
        object Map(Type messageType, HttpRequest context);
    }
}