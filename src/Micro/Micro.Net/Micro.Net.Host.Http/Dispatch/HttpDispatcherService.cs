using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Micro.Net.Abstractions.Messages.Dispatch;
using Microsoft.Extensions.Logging;

namespace Micro.Net.Host.Http
{
    public class HttpDispatcherService : DispatcherService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IFactory<DispatchOptions> _optionFactory;
        private readonly ILogger<HttpDispatcherService> _logger;

        public HttpDispatcherService(IHttpClientFactory clientFactory, IFactory<DispatchOptions> optionFactory, ILogger<HttpDispatcherService> logger)
        {
            _clientFactory = clientFactory;
            _optionFactory = optionFactory;
            _logger = logger;
        }

        private IDictionary<(Type requestType, Type responseType), (Uri uri, string path, HttpMethod verb, RemoteEndpointOptions opts)> _mappings 
            = new Dictionary<(Type requestType, Type responseType), (Uri uri, string path, HttpMethod verb, RemoteEndpointOptions opts)>();

        public async Task<TResponse> Dispatch<TRequest, TResponse>(TRequest message, Action<DispatchOptions> opts)
        {
            (Uri uri, string path, HttpMethod verb, RemoteEndpointOptions remoteOpts) = _mappings[(typeof(TRequest), typeof(TResponse))];

            DispatchOptions options = _optionFactory.Create();

            opts(options);

            HttpResponseMessage response;

            HttpRequestMessage request = new HttpRequestMessage(verb, new Uri(uri, path));

            request.Content = JsonContent.Create<TRequest>(message);



            using (HttpClient client = _clientFactory.CreateClient())
            {
                try
                {
                    response = await client.SendAsync(request);
                }
                catch (HttpRequestException)
                {
                    throw DispatchException.ConnectionFail;
                }
            }

            if (!response.IsSuccessStatusCode)
            {
                if (options.ThrowOnFailure)
                {
                    Exception ex = DispatchException.ConfigurationRelatedError;

                    ex.Data["StatusCode"] = response.StatusCode;

                    throw ex;
                }
                else
                {
                    return default;
                }
            }

            if (typeof(TResponse) == typeof(ValueTuple))
            {
                return default;
            }

            try
            {
                TResponse respMsg = await response.Content.ReadFromJsonAsync<TResponse>();

                return respMsg;
            }
            catch (Exception)
            {
                if (options.ThrowOnFailure)
                {
                    Exception ex = DispatchException.ConfigurationRelatedError;

                    ex.Data["Reason"] = "Deserialization type mismatch.";

                    throw ex;
                }
                else
                {
                    return default;
                }
            }
        }

        public async Task Dispatch<TMessage>(TMessage message, Action<DispatchOptions> opts)
        {
            await Dispatch<TMessage, ValueTuple>(message, opts).ContinueWith(t => {});
        }

        public bool CanHandle<TRequest, TResponse>()
        {
            return _mappings.ContainsKey((typeof(TRequest), typeof(TResponse)));
        }

        public bool CanHandle<TMessage>()
        {
            return CanHandle<TMessage, ValueTuple>();
        }

        public async Task Initialize(CancellationToken cancellationToken)
        {
            
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            
        }
    }
}