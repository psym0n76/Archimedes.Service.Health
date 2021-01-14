using System;
using System.Threading;
using System.Threading.Tasks;
using Archimedes.Service.Health.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Archimedes.Service.Health
{
    public class HealthServiceStrategy : BackgroundService
    {

        private readonly IHttpStrategyClient _httpClient;
        private readonly IHealthDataStore _healthDataStore;
        private readonly ILogger<HealthServiceStrategy> _logger;

        public HealthServiceStrategy(IHttpStrategyClient httpClient, IHealthDataStore healthDataStore, ILogger<HealthServiceStrategy> logger)
        {
            _httpClient = httpClient;
            _healthDataStore = healthDataStore;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Running HealthServiceStrategy");

            while (true)
            {
                try
                {
                    stoppingToken.ThrowIfCancellationRequested();
                    await UpdateStrategyHealth();
                }
                catch (OperationCanceledException ox)
                {
                    _logger.LogError($"Cancellation Invoked {ox.Message} \n\nRetry after 5 secs");
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error found in HealthServiceStrategy: {e.Message} {e.StackTrace}");
                }

                Thread.Sleep(15000);
            }
        }

        private async Task UpdateStrategyHealth()
        {
            var response = await _httpClient.GetStrategyHealth();
            _healthDataStore.Update(response);
        }
    }
}