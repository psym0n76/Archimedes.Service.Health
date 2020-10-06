﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Archimedes.Library.Domain;
using Archimedes.Library.Message.Dto;
using Microsoft.Extensions.Options;

namespace Archimedes.Service.Ui.Http
{
    public class HttpRabbitClient : IHttpRabbitClient
    {
        private readonly HttpClient _client;

        //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.1

        public HttpRabbitClient(IOptions<Config> config, HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri($"http://{config.Value.RabbitHost}:1{config.Value.RabbitPort}");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _client = httpClient;
        }

        public async Task<HealthMonitorDto> GetHealth()
        {
            var response = await _client.GetAsync("/#/");

            var health = new HealthMonitorDto()
            {
                Url = response.RequestMessage.RequestUri.ToString(),
                LastUpdated = DateTime.Now,
                AppName = $"Rabbit",
                Version = "1.0"
            };

            if (!response.IsSuccessStatusCode)
            {
                health.Status = false;
                health.StatusMessage = response.ReasonPhrase;
                return health;
            }

            health.Status = true;
            health.StatusMessage = response.ReasonPhrase;
            health.LastActive = DateTime.Now;

            return health;
        }
    }
}
