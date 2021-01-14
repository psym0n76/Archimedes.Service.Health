using System;
using System.Threading;
using System.Threading.Tasks;
using Archimedes.Service.Health.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Archimedes.Service.Health
{
    public class HealthServiceBroker : BackgroundService
    {

        private readonly IHttpBrokerClient _httpClient;
        private readonly IHealthDataStore _healthDataStore;
        private readonly ILogger<HealthServiceBroker> _logger;

        public HealthServiceBroker(IHttpBrokerClient httpClient, IHealthDataStore healthDataStore,
            ILogger<HealthServiceBroker> logger)
        {
            _httpClient = httpClient;
            _healthDataStore = healthDataStore;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Running HealthServiceBroker");

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
                    _logger.LogError($"Error found in HealthServiceBroker: {e.Message} {e.StackTrace}");
                }

                Thread.Sleep(5000);
            }
        }

        private async Task UpdateUiHealth()
        {
            var response = await _httpClient.GetHealth();
            _healthDataStore.Update(response);
        }
    }
}