using System;
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
        private readonly Config _config;

        //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.1

        public HttpRabbitClient(IOptions<Config> config, HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri($"http://{config.Value.RabbitHost}:1{config.Value.RabbitPort}");
            _client = httpClient;
            _config = config.Value;
        }

        public async Task<HealthMonitorDto> GetHealth()
        {
            //todo needs refactoring
            var health = new HealthMonitorDto()
            {
                LastUpdated = DateTime.Now,
                AppName = "Archimedes.Rabbit",
                Version = "1.0.0",
                Url = $"http://{_config.RabbitHost}:1{_config.RabbitPort}/"
            };

            try
            {
                var response = await _client.GetAsync("");

                health.Url = response.RequestMessage.RequestUri.ToString();

                if (!response.IsSuccessStatusCode)
                {
                    health.Status = false;
                    health.StatusMessage = response.ReasonPhrase;
                    return health;
                }

                health.Status = true;
                health.StatusMessage = response.ReasonPhrase;
                health.LastActive = DateTime.Now;
            }
            catch (Exception e)
            {
                health.Status = false;

                if (e.Message == "No connection could be made because the target machine actively refused it.")
                {
                    health.StatusMessage = "Refused";
                }
            }

            return health;
        }
    }
}
