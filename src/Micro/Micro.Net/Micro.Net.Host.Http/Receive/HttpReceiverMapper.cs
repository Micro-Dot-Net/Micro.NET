using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Micro.Net.Abstractions.Contexts;
using Micro.Net.Abstractions.Exceptions;
using Micro.Net.Abstractions.Messages.Receive;
using Micro.Net.Abstractions.Processing;
using Newtonsoft.Json.Linq;

namespace Micro.Net.Host.Http
{
    public class HttpReceiverMapper : IReceiverMapper<HttpReceiveContext>
    {
        private readonly HttpReceiveDirectory _directory;
        private readonly IServiceProvider _provider;

        public HttpReceiverMapper(HttpReceiveDirectory directory, IServiceProvider provider)
        {
            _directory = directory;
            _provider = provider;
        }
        
        public async Task Step(HttpReceiveContext context, PipelineDelegate<HttpReceiveContext> next)
        {
            if (_directory.Reverse.TryGetValue(context.Request.Origin.AbsolutePath, out (Type, Type) mappings))
            {
                Type contextType = typeof(MappedReceiveContext<,>).MakeGenericType(mappings.Item1, mappings.Item2);
                Type pipelineType = typeof(IPipelineHead<>).MakeGenericType(contextType);
                Type reqContextType = typeof(MappedReceiveRequestContext<>).MakeGenericType(mappings.Item1);
                Type respContextType = typeof(MappedReceiveResponseContext<>).MakeGenericType(mappings.Item2);

                object contextObj = Activator.CreateInstance(contextType);
                object pipeline = _provider.GetService(pipelineType);
                object reqContext = Activator.CreateInstance(reqContextType);
                object respContext = Activator.CreateInstance(respContextType);

                object reqObj = context.Request.Body.ToObject(mappings.Item1);
                object respObj = context.Response.Body.ToObject(mappings.Item2);

                contextType.GetProperty(nameof(MappedReceiveContext<object,object>.Request)).SetValue(contextObj,reqContext);
                contextType.GetProperty(nameof(MappedReceiveContext<object,object>.Response)).SetValue(contextObj,respContext);
                
                reqContextType.GetProperty(nameof(MappedReceiveRequestContext<object>.Body)).SetValue(reqContext,reqObj);
                reqContextType.GetProperty(nameof(MappedReceiveRequestContext<object>.Headers)).SetValue(reqContext,context.Request.Headers);
                reqContextType.GetProperty(nameof(MappedReceiveRequestContext<object>.Origin)).SetValue(reqContext,context.Request.Origin);
                reqContextType.GetProperty(nameof(MappedReceiveResponseContext<object>.Body)).SetValue(respContext,reqObj);
                reqContextType.GetProperty(nameof(MappedReceiveResponseContext<object>.Headers)).SetValue(respContext,context.Response.Headers);

                await (Task)pipelineType.GetMethod(nameof(IPipelineHead<object>.Execute)).Invoke(pipeline, new []{contextObj});

                Type httpCtxType = typeof(HttpReceiveContext);
                Type httpCtxRespType = typeof(HttpReceiveResponseContext);
                
                context.Response.Body = JObject.FromObject(respContextType.GetProperty(nameof(MappedReceiveResponseContext<object>.Body)).GetValue(respObj));
                context.Response.Headers = (IDictionary<string,string>)respContextType.GetProperty(nameof(MappedReceiveResponseContext<object>.Headers)).GetValue(respObj);
                
                return;
            }
            
            throw MicroReceiverException.NoMapping;
        }
    }
}