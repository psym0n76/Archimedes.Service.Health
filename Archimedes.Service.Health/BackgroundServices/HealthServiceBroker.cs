using System;
using System.Threading;
using System.Threading.Tasks;
using Archimedes.Service.Ui.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Archimedes.Service.Health
{
    public class HealthServiceBroker : BackgroundService
    {

        private readonly IHttpBrokerClient _httpClient;
        private readonly IHealthDataStore _healthDataStore;
        private readonly ILogger<HealthServiceBroker> _logger;

        public HealthServiceBroker(IHttpBrokerClient httpClient, IHealthDataStore healthDataStore, ILogger<HealthServiceBroker> logger)
        {
            _httpClient = httpClient;
            _healthDataStore = healthDataStore;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                    stoppingToken.ThrowIfCancellationRequested();
                    _logger.LogInformation($"Running HealthServiceBroker");
                    await UpdateUiHealth();
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error found in HealthServiceBroker: {e.Message} {e.StackTrace}");
                }

                Thread.Sleep(30000);
            }
        }

        private async Task UpdateUiHealth()
        {
            var response = await _httpClient.GetHealth();
            _healthDataStore.Update(response);
        }
    }
}