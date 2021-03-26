using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Micro.Net.Dispatch.Http
{
    public class HttpDispatcher : IDispatcher
    {
        private readonly HttpDispatcherConfiguration _config;
        private readonly HttpClient _client;

        public HttpDispatcher(HttpDispatcherConfiguration config, HttpClient client)
        {
            _config = config;
            _client = client;
        }

        public IEnumerable<(Type, Type)> Available => _config.Routes.Keys;
        public ISet<DispatcherFeature> Features => new HashSet<DispatcherFeature> {DispatcherFeature.Replies};

        public async Task<TResponse> Handle<TRequest, TResponse>(TRequest message, DispatchOptions options)
        {
            if (!Available.Contains((typeof(TRequest), typeof(TResponse))))
            {
                throw HttpDispatcherException.NoRouteFound(typeof(TRequest), typeof(TResponse));
            }

            (Uri route, HttpMethod verb) = _config.Routes[(typeof(TRequest), typeof(TResponse))];

            HttpResponseMessage response;

            HttpRequestMessage request = new HttpRequestMessage(verb, route);

            request.Content = JsonContent.Create<TRequest>(message);

            request.Headers.Add("Source", $"machine://{Environment.MachineName}");

            foreach (KeyValuePair<string, string[]> keyValuePair in _config.DefaultHeaders)
            {
                request.Headers.Add(keyValuePair.Key, keyValuePair.Value);
            }

            foreach ((string, string) header in options.Headers)
            {
                request.Headers.Add(header.Item1, header.Item2);
            }
            
            try
            {
                response = await _client.SendAsync(request);
            }
            catch (HttpRequestException)
            {
                throw HttpDispatcherException.ConnectionFail;
            }

            if (!response.IsSuccessStatusCode)
            {
                if (options.ThrowOnFailure)
                {
                    Exception ex = HttpDispatcherException.ConfigurationRelatedError;

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
                    Exception ex = HttpDispatcherException.ConfigurationRelatedError;

                    ex.Data["Reason"] = "Deserialization type mismatch.";

                    throw ex;
                }
                else
                {
                    return default;
                }
            }
        }
    }
}
