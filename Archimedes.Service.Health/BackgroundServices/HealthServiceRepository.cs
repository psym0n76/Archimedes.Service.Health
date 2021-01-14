using System;
using System.Threading;
using System.Threading.Tasks;
using Archimedes.Service.Health.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Archimedes.Service.Health
{
    public class HealthServiceRepository : BackgroundService
    {

        private readonly IHttpRepositoryClient _httpClient;
        private readonly IHealthDataStore _healthDataStore;
        private readonly ILogger<HealthServiceRepository> _logger;

        public HealthServiceRepository(IHttpRepositoryClient httpClient, IHealthDataStore healthDataStore, ILogger<HealthServiceRepository> logger)
        {
            _httpClient = httpClient;
            _healthDataStore = healthDataStore;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Running HealthServiceRepository");

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
                    _logger.LogError($"Error found in HealthServiceRepository: {e.Message} {e.StackTrace}");
                }

                Thread.Sleep(15000);
            }
        }

        private async Task UpdateUiHealth()
        {
            var response = await _httpClient.GetRepositoryHealth();
            _healthDataStore.Update(response);
        }
    }
}