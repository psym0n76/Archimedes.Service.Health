using System;
using System.Threading;
using System.Threading.Tasks;
using Archimedes.Service.Ui.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Archimedes.Service.Health
{
    public class HealthServiceUi : BackgroundService
    {

        private readonly IHttpUiClient _httpClient;
        private readonly IHealthDataStore _healthDataStore;
        private readonly ILogger<HealthServiceUi> _logger;

        public HealthServiceUi(IHttpUiClient httpClient, IHealthDataStore healthDataStore, ILogger<HealthServiceUi> logger)
        {
            _httpClient = httpClient;
            _healthDataStore = healthDataStore;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Running HealthServiceUi");

            while (true)
            {
                try
                {
                    stoppingToken.ThrowIfCancellationRequested();
                    await UpdateUiHealth();
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error found in HealthServiceUi: {e.Message} {e.StackTrace}");
                }

                Thread.Sleep(15000);
            }
        }

        private async Task UpdateUiHealth()
        {
            var response = await _httpClient.GetUiHealth();
            _healthDataStore.Update(response);
        }
    }
}