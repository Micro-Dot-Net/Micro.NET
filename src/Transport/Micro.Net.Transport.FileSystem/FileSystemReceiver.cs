using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using ChinhDo.Transactions;
using Micro.Net.Abstractions;
using Micro.Net.Core.Abstractions.Pipeline;
using Micro.Net.Exceptions;
using Micro.Net.Receive;
using Micro.Net.Transport.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Micro.Net.Transport.FileSystem
{
    public class FileSystemReceiver : GenericReceiverBase
    {
        private readonly FileSystemReceiveConfiguration _config;
        private readonly ISerializerCollection _serializerCollection;
        private readonly IContextFactory _contextFactory;
        private readonly ICollection<FileSystemWatcher> _watchers;

        public FileSystemReceiver(IPipeChannel receivePipeFactory, FileSystemReceiveConfiguration config, ISerializerCollection serializerCollection, IContextFactory contextFactory) : base(receivePipeFactory)
        {
            _config = config;
            _serializerCollection = serializerCollection;
            _contextFactory = contextFactory;

            ISet<string> missingSerializers = new HashSet<string>();

            foreach (KeyValuePair<Type, MessageProcessConfiguration> mapping in _config.Mappings)
            {
                if (!Directory.Exists(mapping.Value.RequestDir))
                {
                    Directory.CreateDirectory(mapping.Value.RequestDir);
                }

                if (!Directory.Exists(mapping.Value.ResponseDir) && mapping.Value.ResponseType != typeof(ValueTuple))
                {
                    Directory.CreateDirectory(mapping.Value.ResponseDir);
                }

                FileSystemWatcher watcher = new FileSystemWatcher(mapping.Value.RequestDir);

                watcher.NotifyFilter = NotifyFilters.CreationTime;

                if (!string.IsNullOrWhiteSpace(mapping.Value.RequestFilter))
                {
                    foreach (string filter in mapping.Value.RequestFilter.Split(';'))
                    {
                        watcher.Filters.Add(filter);
                    }
                }

                if (_serializerCollection.Get(mapping.Value.RequestSerializer) != null)
                {
                    missingSerializers.Add(mapping.Value.ResponseSerializer);
                }

                if (mapping.Value.ResponseType != typeof(ValueTuple) && _serializerCollection.Get(mapping.Value.ResponseSerializer) != null)
                {
                    missingSerializers.Add(mapping.Value.ResponseSerializer);
                }

                Type rcvType = mapping.Key;

                watcher.Created += (sender, e) => OnCreated(sender, e, rcvType);
            }

            if (missingSerializers.Any())
            {
                throw MicroConfigurationException
                    .MissingRegistrations(missingSerializers.ToDictionary(x => x, y => typeof(ISerializer).FullName));
            }
        }

        public override async Task Start(CancellationToken cancellationToken)
        {
            foreach (FileSystemWatcher watcher in _watchers)
            {
                watcher.EnableRaisingEvents = true;
            }
        }

        private void OnCreated(object sender, FileSystemEventArgs e, Type rcvType)
        {
            MessageProcessConfiguration msgConfig = _config.Mappings[rcvType];

            MethodInfo method = this.GetType().GetMethod(nameof(_handle)).MakeGenericMethod(rcvType, msgConfig.ResponseType);

            Task.Run( () => method.Invoke(this, new object[]{e.FullPath}));
        }

        private async Task _handle<TRequest, TResponse>(string filePath)
        {
            MessageProcessConfiguration msgConfig = _config.Mappings[typeof(TRequest)];

            ISerializer serializer = _serializerCollection.Get(msgConfig.RequestSerializer);

            Envelope<TRequest> message;

            await using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    message = serializer.Materialize<Envelope<TRequest>>(sr.ReadToEnd());
                }
            }

            IReceiveContext<TRequest, TResponse> context;
            
            if(!_contextFactory.TryCreate(out context))
            {
                throw MicroConfigurationException.MissingRegistrations(new Dictionary<string, string> { {$"Factory:{nameof(IReceiveContext<TRequest,TResponse>)}", nameof(IContextSubFactory)} });
            }

            context.Request.Payload = message.Request;
            context.Request.Headers = message.Headers;

            try
            {
                await base.Dispatch(context);
            }
            catch(Exception ex)
            {
                context.SetFault(ex);
            }

            if ((context.IsFaulted || context.IsTerminated) && msgConfig.KeepSkips)
            {
                string name = Path.GetFileName(filePath);

                File.Move(filePath, Path.Combine(msgConfig.SkipDirectory, name));

                using (FileStream fs = File.Create(Path.Combine(msgConfig.SkipDirectory, name + $"_{context.Status}.log")))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        if (context.TryGetFault(out Exception ex))
                        {
                            await sw.WriteAsync(ex.ToString());
                        }

                        if (context.TryGetTerminate(out string reason, out IDictionary<string, string> auxData))
                        {
                            JObject obj = new JObject();
                            obj["reason"] = reason;
                            obj["aux_data"] = JObject.FromObject(auxData);

                            await sw.WriteAsync(obj.ToString());
                        }
                    }
                }
            }

            if (context.IsResolved)
            {
                string name = Path.GetFileName(filePath);

                if (typeof(TResponse) != typeof(ValueTuple))
                {
                    serializer = _serializerCollection.Get(msgConfig.ResponseSerializer);

                    Envelope<TResponse> envelope = new Envelope<TResponse>()
                    {
                        Headers = context.Response.Headers,
                        Request = context.Response.Payload
                    };

                    using (FileStream fs = File.Create(Path.Combine(msgConfig.ResponseDir, name)))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            await sw.WriteLineAsync(serializer.Serialize(envelope));
                        }
                    }
                }

                if (msgConfig.KeepProcessed)
                {
                    File.Move(filePath, Path.Combine(msgConfig.ProcessedDirectory, name));
                }
            }
        }

        public override async Task Stop(CancellationToken cancellationToken)
        {
            foreach(FileSystemWatcher watcher in _watchers)
            {
                watcher.Dispose();
            }
        }
    }
}