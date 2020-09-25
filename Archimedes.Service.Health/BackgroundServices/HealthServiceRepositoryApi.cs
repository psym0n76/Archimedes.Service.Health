using System;
using System.Threading;
using System.Threading.Tasks;
using Archimedes.Service.Ui.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Archimedes.Service.Health
{
    public class HealthServiceRepositoryApi : BackgroundService
    {

        private readonly IHttpRepositoryApiClient _httpClient;
        private readonly IHealthDataStore _healthDataStore;
        private readonly ILogger<HealthServiceRepositoryApi> _logger;

        public HealthServiceRepositoryApi(IHttpRepositoryApiClient httpClient, IHealthDataStore healthDataStore, ILogger<HealthServiceRepositoryApi> logger)
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
                    _logger.LogInformation($"Running HealthServiceRepositoryApi");
                    await UpdateUiHealth();
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error found in HealthServiceRepositoryApi: {e.Message} {e.StackTrace}");
                }

                Thread.Sleep(20000);
            }
        }

        private async Task UpdateUiHealth()
        {
            var response = await _httpClient.GetRepositoryApiHealth();
            _healthDataStore.Update(response);
        }
    }
}