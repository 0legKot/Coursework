using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Retry;

namespace ConsulServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        readonly IConfiguration _config;
        readonly HttpClient _apiClient;
        AsyncRetryPolicy _serverRetryPolicy;
        int _currentConfigIndex;
        List<Uri> _serverUrls;
        public ServerController(IConfiguration config)
        {
            _config = config;
            _apiClient = new HttpClient();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [HttpGet]
        [HttpGet("get/{query}")]
        public async Task<string> AllGets([FromRoute] string query)
        {
            await Initialize();
             return await _serverRetryPolicy.ExecuteAsync(async () =>
             {
                 var serverUrl = _serverUrls[_currentConfigIndex];
                 var requestPath = $"{serverUrl}api/values/{query.Replace('|','/')}";
                 var response = await _apiClient.GetAsync(requestPath).ConfigureAwait(false);
                 return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
             });
        }

        public async Task Initialize()
        {
            var consulClient = new ConsulClient(c =>
            {
                var uri = new Uri(_config["ConsulConfig:Address"]);
                c.Address = uri;
            });
            _serverUrls = new List<Uri>();
            var services = await consulClient.Agent.Services();
            foreach (var service in services.Response)
            {
                var isDemoApi = service.Value.Tags.Any(t => t == "Consul");
                if (isDemoApi)
                {
                    var serviceUri = new Uri($"{service.Value.Address}:{service.Value.Port}");
                    _serverUrls.Add(serviceUri);
                }
            }
            var retries = _serverUrls.Count * 2 - 1;
            _serverRetryPolicy = Policy.Handle<HttpRequestException>()
                .RetryAsync(retries, (exception, retryCount) =>
                {
                    ChooseNextServer(retryCount);
                });
        }
        private void ChooseNextServer(int retryCount)
        {
            if (retryCount % 2 == 0)
            {
                _currentConfigIndex++;

                if (_currentConfigIndex > _serverUrls.Count - 1)
                    _currentConfigIndex = 0;
            }
        }
    }
}
