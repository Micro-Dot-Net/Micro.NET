using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Micro.Net.Dispatch;

namespace Micro.Net.Transport.Http
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

        public async Task Handle<TRequest, TResponse>(IDispatchContext<TRequest,TResponse> messageContext) where TRequest : IContract<TResponse>
        {
            if (!Available.Contains((typeof(TRequest), typeof(TResponse))))
            {
                throw HttpDispatcherException.NoRouteFound(typeof(TRequest), typeof(TResponse));
            }

            (Uri route, HttpMethod verb) = _config.Routes[(typeof(TRequest), typeof(TResponse))];

            HttpResponseMessage response;

            HttpRequestMessage request = new HttpRequestMessage(verb, route);

            request.Content = JsonContent.Create<TRequest>(messageContext.Request.Payload);

            request.Headers.Add("Source", $"machine://{Environment.MachineName}");

            foreach (KeyValuePair<string, string[]> keyValuePair in _config.DefaultHeaders)
            {
                request.Headers.Add(keyValuePair.Key, keyValuePair.Value);
            }

            foreach (KeyValuePair<string, string[]> header in messageContext.Request.Headers)
            {
                request.Headers.Add(header.Key, header.Value);
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
                Exception ex = HttpDispatcherException.ConfigurationRelatedError;

                ex.Data["StatusCode"] = response.StatusCode;

                messageContext.SetFault(ex);
            }

            if (typeof(TResponse) == typeof(ValueTuple))
            {
                messageContext.SetResolve();

                return;
            }

            try
            {
                TResponse respMsg = await response.Content.ReadFromJsonAsync<TResponse>();

                messageContext.Response.Payload = respMsg;

                messageContext.SetResolve();
            }
            catch (Exception)
            {
                 Exception ex = HttpDispatcherException.ConfigurationRelatedError;

                    ex.Data["Reason"] = "Deserialization type mismatch.";

                    messageContext.SetFault(ex);
            }
        }
    }
}
