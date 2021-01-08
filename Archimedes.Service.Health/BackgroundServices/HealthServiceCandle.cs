using System;
using System.Threading;
using System.Threading.Tasks;
using Archimedes.Service.Ui.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Archimedes.Service.Health
{
    public class HealthServiceCandle : BackgroundService
    {

        private readonly IHttpCandleClient _httpClient;
        private readonly IHealthDataStore _healthDataStore;
        private readonly ILogger<HealthServiceCandle> _logger;

        public HealthServiceCandle(IHttpCandleClient httpClient, IHealthDataStore healthDataStore, ILogger<HealthServiceCandle> logger)
        {
            _httpClient = httpClient;
            _healthDataStore = healthDataStore;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Running HealthServiceCandle");

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
                    _logger.LogError($"Error found in HealthServiceCandle: {e.Message} {e.StackTrace}");
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