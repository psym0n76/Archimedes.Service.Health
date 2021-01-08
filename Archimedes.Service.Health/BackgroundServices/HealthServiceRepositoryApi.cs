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
            _logger.LogInformation($"Running HealthServiceRepositoryApi");

            while (true)
            {
                try
                {
                    stoppingToken.ThrowIfCancellationRequested();
                    await UpdateUiHealth();
                }
                catch (OperationCanceledException ox)
                {
                    _logger.LogError($"Cancellation Invoked {ox.Message} \n\nRetry after 5 secs");
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error found in HealthServiceRepositoryApi: {e.Message} {e.StackTrace}");
                }

                Thread.Sleep(15000);
            }
        }

        private async Task UpdateUiHealth()
        {
            var response = await _httpClient.GetRepositoryApiHealth();
            _healthDataStore.Update(response);
        }
    }
}