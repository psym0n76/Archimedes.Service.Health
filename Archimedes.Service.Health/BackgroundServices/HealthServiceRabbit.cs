using System;
using System.Threading;
using System.Threading.Tasks;
using Archimedes.Service.Ui.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Archimedes.Service.Health
{
    public class HealthServiceRabbit : BackgroundService
    {
        private readonly IHttpRabbitClient _httpClient;
        private readonly IHealthDataStore _healthDataStore;
        private readonly ILogger<HealthServiceRabbit> _logger;

        public HealthServiceRabbit(IHttpRabbitClient httpClient, IHealthDataStore healthDataStore, ILogger<HealthServiceRabbit> logger)
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
                    _logger.LogInformation($"Running HealthServiceRabbit");
                    await UpdateUiHealth();
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error found in HealthServiceRabbit: {e.Message} {e.StackTrace}");
                }

                Thread.Sleep(15000);
            }
        }

        private async Task UpdateUiHealth()
        {
            var response = await _httpClient.GetHealth();
            _healthDataStore.Update(response);
        }
    }
}