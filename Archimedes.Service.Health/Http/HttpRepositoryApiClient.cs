using System;
using System.Net.Http;
using System.Threading.Tasks;
using Archimedes.Library.Domain;
using Archimedes.Library.Message.Dto;
using Microsoft.Extensions.Options;

namespace Archimedes.Service.Ui.Http
{
    public class HttpRepositoryApiClient : IHttpRepositoryApiClient
    {
        private readonly HttpClient _client;
        private readonly IHealthResponse _healthResponse;

        //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.1

        public HttpRepositoryApiClient(IOptions<Config> config, HttpClient httpClient, IHealthResponse healthResponse)
        {
            httpClient.BaseAddress = new Uri($"{config.Value.ApiRepositoryUrl}");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _client = httpClient;
            _healthResponse = healthResponse;
        }

        public async Task<HealthMonitorDto> GetRepositoryApiHealth()
        {
            var response = await _client.GetAsync("health");

            var healthResponse =  await _healthResponse.Build(response);
            return healthResponse;
        }
    }
}