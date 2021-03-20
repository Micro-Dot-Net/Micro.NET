using Micro.Net.Abstractions.Contexts;

namespace Micro.Net.Host.Http
{
    public class HttpReceiveContext : ReceiveContext
    {
        public override ReceiveRequestContext Request => HttpRequest;
        public override ReceiveResponseContext Response => HttpResponse;
        
        public HttpReceiveRequestContext HttpRequest { get; set; }
        public HttpReceiveResponseContext HttpResponse { get; set; }
    }
}