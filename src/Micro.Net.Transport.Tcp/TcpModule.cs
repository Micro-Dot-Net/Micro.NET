using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;

namespace Micro.Net.Transport.Tcp
{
    public class TcpModule
    {
        private readonly IPEndPoint _endpoint;
        private readonly ModuleMode _mode;
        private readonly ModuleBehavior.Connection _connBehavior;
        private readonly ModuleBehavior.Response _respBehavior;
        private readonly JsonSerializer _serializer;
        private readonly SecureTcpOptions _secureOptions;
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private readonly BlockingCollection<(JObject, Action<JObject>)> _outbound = new BlockingCollection<(JObject, Action<JObject>)>();

        private readonly ConcurrentDictionary<Guid, Action<JObject>> _inbound = new ConcurrentDictionary<Guid, Action<JObject>>();

        public TcpModule(IPEndPoint endpoint, ModuleMode mode, ModuleBehavior.Connection connBehavior, ModuleBehavior.Response respBehavior, JsonSerializer serializer, SecureTcpOptions secureOptions)
        {
            _endpoint = endpoint;
            _mode = mode;
            _connBehavior = connBehavior;
            _respBehavior = respBehavior;
            _serializer = serializer;
            _secureOptions = secureOptions;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            switch (_mode)
            {
                case ModuleMode.Receive:
                    new Thread(async () => await _listenThread()) { IsBackground = true }.Start();
                    break;
                case ModuleMode.Dispatch:
                    await _dspStart();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private async Task _listenThread()
        {
            CancellationToken token = _tokenSource.Token;

            TcpListener listener = new TcpListener(_endpoint);

            listener.Start();

            token.Register(() => listener.Stop());

            while (!_tokenSource.IsCancellationRequested)
            {
                try
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();

                    Stream stream = client.GetStream();

                    if (_secureOptions.UseSsl)
                    {
                        stream = new SslStream(stream, false, _remoteAuthCallback, _localSelectCallback);
                    }

                    BlockingCollection<(JObject, Action<JObject>)> outbound = new BlockingCollection<(Newtonsoft.Json.Linq.JObject, System.Action<Newtonsoft.Json.Linq.JObject>)>();

                    new Thread(() => _sendPump_Rcv(outbound, stream, token)) { IsBackground = true }.Start();
                    new Thread(() => _rcvPump(stream, outbound, token)) { IsBackground = true }.Start();
                }
                catch
                {

                }
            }
        }

        private async Task _dspStart()
        {
            switch (_connBehavior)
            {
                case ModuleBehavior.Connection.AlwaysOpen:
                    new Thread(() => _sendPump_Open(_tokenSource.Token)) { IsBackground = true }.Start();
                    break;
                //case ModuleBehavior.Connection.CloseWhenIdle:
                //    new Thread(() => _sendPump_IdleClose(_tokenSource.Token)) { IsBackground = true }.Start();
                //    break;
                case ModuleBehavior.Connection.OpenPerRequest:
                    new Thread(() => _sendPump_PerReq(_tokenSource.Token)) { IsBackground = true }.Start();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            _tokenSource.Cancel(false);
        }

        public event Action<MessageReceivedArgs> OnReceive;

        public async Task Send(JObject obj, Action<JObject> replyCallback = null)
        {
            _outbound.TryAdd((obj, replyCallback));
        }

        private void _sendPump_Open(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                CancellationTokenSource source = CancellationTokenSource.CreateLinkedTokenSource(token);

                try
                {
                    TcpClient client = new TcpClient();

                    client.Connect(_endpoint);

                    Stream stream = client.GetStream();

                    if (_secureOptions.UseSsl)
                    {
                        stream = new SslStream(stream, false, _remoteAuthCallback, _localSelectCallback);
                    }

                    BsonDataWriter writer = new BsonDataWriter(stream) { CloseOutput = false };

                    new Thread(() => _rcvPump(stream, _outbound, source.Token)) { IsBackground = true }.Start();

                    try
                    {
                        foreach ((JObject obj, Action<JObject> reply) in _outbound.GetConsumingEnumerable(token))
                        {
                            Guid messageId = Guid.NewGuid();

                            if (reply != null)
                            {
                                obj["Message-Id"] = messageId;

                                _inbound.TryAdd(messageId, reply);
                            }

                            _serializer.Serialize(writer, obj);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        writer.Close();
                    }
                }
                catch (Exception ex)
                {
                    source.Cancel();
                }
            }
        }

        //private void _sendPump_IdleClose(CancellationToken token)
        //{
        //    while (!token.IsCancellationRequested)
        //    {

        //    }
        //}


        private void _sendPump_PerReq(CancellationToken token)
        {
            foreach ((JObject obj, Action<JObject> callback) in _outbound)
            {
                Guid messageId = Guid.NewGuid();

                obj["Message-Id"] = messageId;

                TcpClient client = new TcpClient();

                client.Connect(_endpoint);

                Stream stream = client.GetStream();

                if (_secureOptions.UseSsl)
                {
                    stream = new SslStream(stream, false, _remoteAuthCallback, _localSelectCallback);
                }

                using (BsonDataWriter writer = new BsonDataWriter(stream) { CloseOutput = false })
                {
                    _serializer.Serialize(writer, obj);
                }

                if (_respBehavior == ModuleBehavior.Response.RequireAck)
                {
                    using (BsonDataReader reader = new BsonDataReader(stream) { CloseInput = false })
                    {
                        object rcvobj = _serializer.Deserialize(reader);

                        JObject jObj = JObject.FromObject(rcvobj, _serializer);

                        if (callback != null)
                        {
                            callback?.Invoke(jObj);
                        }
                        else
                        {
                            MessageReceivedArgs args = new MessageReceivedArgs()
                                { Message = jObj };

                            OnReceive?.Invoke(args);
                        }
                    }
                }

                client.Close();
            }
        }

        private void _rcvPump(Stream stream, BlockingCollection<(JObject, Action<JObject>)> outbound, CancellationToken token)
        {
            using (BsonDataReader reader = new BsonDataReader(stream) { CloseInput = false, SupportMultipleContent = true })
            {
                token.Register(() => { reader.Close(); });

                do
                {
                    object obj = _serializer.Deserialize(reader);

                    JObject jObj = JObject.FromObject(obj, _serializer);

                    Guid messageId = jObj["Message-Id"]?.ToObject<Guid>() ?? Guid.Empty;

                    if (messageId != default && _inbound.TryRemove(messageId, out Action<JObject> callback))
                    {
                        callback?.Invoke(jObj);
                    }
                    else
                    {
                        MessageReceivedArgs args = new MessageReceivedArgs()
                        {
                            Message = jObj,
                            Reply = replyObj => {
                                if (messageId != default)
                                {
                                    replyObj["Message-Id"] = messageId;
                                }

                                outbound.Add((replyObj, null), token);
                            }
                        };

                        OnReceive?.Invoke(args);
                    }


                } while (reader.Read());
            }
        }

        private void _sendPump_Rcv(BlockingCollection<(JObject, Action<JObject>)> objCollection, Stream stream, CancellationToken token)
        {
            using (BsonDataWriter writer = new BsonDataWriter(stream) { CloseOutput = false })
            {
                foreach ((JObject obj, Action<JObject> reply) in objCollection.GetConsumingEnumerable(token))
                {
                    _serializer.Serialize(writer, obj);
                }
            }
        }

        private X509Certificate _localSelectCallback(object sender, string targethost, X509CertificateCollection localcertificates, X509Certificate remotecertificate, string[] acceptableissuers)
        {
            return _secureOptions.Certificate;
        }

        private bool _remoteAuthCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            X509Certificate2 certificate2 = new X509Certificate2(certificate);

            return chain.Build(certificate2) && _secureOptions.AcceptThumbprints.Contains(certificate2.Thumbprint);
        }

        public enum ModuleMode
        {
            Receive,
            Dispatch
        }

        public static class ModuleBehavior
        {
            public enum Connection
            {
                AlwaysOpen,
                //CloseWhenIdle,
                OpenPerRequest
            }

            public enum Response
            {
                RequireAck,
                FireForget
            }
        }

        public class MessageReceivedArgs
        {
            public JObject Message { get; init; }
            public Action<JObject> Reply { get; init; }
        }
    }
}