using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Transport;
using Micro.Net.Dispatch;
using Micro.Net.Extensions;

namespace Micro.Net.Transport.FileSystem
{
    public class FileSystemDispatcher : IDispatcher
    {
        private readonly FileSystemDispatchConfiguration _config;
        private readonly ISerializerCollection _serializerCollection;

        public FileSystemDispatcher(FileSystemDispatchConfiguration config, ISerializerCollection serializerCollection)
        {
            _config = config;
            _serializerCollection = serializerCollection;
        }

        public ISet<DispatcherFeature> Features => new HashSet<DispatcherFeature>
            {DispatcherFeature.Async};

        public IEnumerable<(Type, Type)> Available => 
            _config.Mappings.Select(x => (x.Key, x.Value.ResponseType));

        public async Task Handle<TRequest, TResponse>(IDispatchContext<TRequest, TResponse> messageContext) where TRequest : IContract<TResponse>
        {
            MessageProcessConfiguration config = _config.Mappings[typeof(TRequest)];

            Envelope<TRequest> envelope = new Envelope<TRequest>();

            envelope.Headers = messageContext.Request.Headers;
            envelope.Message = messageContext.Request.Payload;

            string fileName = $"{DateTime.Now.ToFileTimeUtc()}_{Guid.NewGuid().EncodeBase64String()}.micro";

            ISerializer serializer = _serializerCollection.Get(config.RequestSerializer);

            try
            {
                using (FileStream fs = File.Create(config.RequestDir))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        string val = serializer.Serialize(envelope);

                        await sw.WriteAsync(val);
                    }
                }
            }
            catch (Exception ex)
            {
                messageContext.SetFault(ex);

                return;
            }

            messageContext.SetResolve();
        }
    }
}