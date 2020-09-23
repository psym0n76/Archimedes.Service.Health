﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Archimedes.Library.Domain;
using Archimedes.Library.Extensions;
using Archimedes.Library.Message.Dto;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Archimedes.Service.Ui.Http
{
    public class HttpUiClient : IHttpUiClient
    {
        private readonly ILogger<HttpUiClient> _logger;
        private readonly HttpClient _client;

        //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.1

        public HttpUiClient(IOptions<Config> config, HttpClient httpClient, ILogger<HttpUiClient> logger)
        {
            httpClient.BaseAddress = new Uri($"{config.Value.UserInterfaceBaseUrl}");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _client = httpClient;
            _logger = logger;
        }


        public async Task<HealthMonitorDto> GetUiHealth()
        {
            var health = new HealthMonitorDto()
            {
                Url = $"{_client.BaseAddress}health",
                LastUpdated = DateTime.Now
            };

            var response = await _client.GetAsync("health");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"GET Failed:  {response.ReasonPhrase} from {_client.BaseAddress}health");
                health.Status = false;
                health.StatusMessage = response.ReasonPhrase;
                return health;
            }

            var healthDto = await response.Content.ReadAsAsync<HealthMonitorDto>();

            health.Status = true;
            health.StatusMessage = response.ReasonPhrase;
            health.Version = healthDto.Version;
            health.LastActive = DateTime.Now;
            health.AppName = healthDto.AppName;
            health.AppName = healthDto.AppName;

            return health;
        }
    }
}