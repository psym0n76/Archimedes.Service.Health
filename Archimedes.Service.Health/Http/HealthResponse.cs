using System;
using System.Net.Http;
using System.Threading.Tasks;
using Archimedes.Library.Extensions;
using Archimedes.Library.Logger;
using Archimedes.Library.Message.Dto;
using Microsoft.Extensions.Logging;

namespace Archimedes.Service.Health.Http
{
    public class HealthResponse : IHealthResponse
    {
        private readonly ILogger<HealthResponse> _logger;
        private readonly BatchLog _batchLog = new();
        private string _logId;

        public HealthResponse(ILogger<HealthResponse> logger)
        {
            _logger = logger;
        }

        public async Task<HealthMonitorDto> Build(HttpResponseMessage response)
        {
            _logId = _batchLog.Start();
            _batchLog.Update(_logId, $"Build HealthResponse");
            
            if (!response.IsSuccessStatusCode)
            {
                if (response.RequestMessage != null)
                    
                    _logger.LogWarning(
                        _batchLog.Print(_logId,
                            $"GET Failed: {response.ReasonPhrase} from {response.RequestMessage.RequestUri}"));

                return new HealthMonitorDto()
                {
                    Status = false,
                    StatusMessage = response.ReasonPhrase,
                    Url = response.RequestMessage.RequestUri.ToString(),
                    LastUpdated = DateTime.Now
                };
            }

            var healthDto = await response.Content.ReadAsAsync<HealthMonitorDto>();
            
            _logger.LogInformation(_batchLog.Print(_logId,$"Response: {response.ReasonPhrase} from {response.RequestMessage.RequestUri.ToString()}"));

            return new HealthMonitorDto()
            {
                Url = response.RequestMessage.RequestUri.ToString(),
                Status = true,
                StatusMessage = response.ReasonPhrase,
                Version = healthDto.Version,
                LastActive = DateTime.Now,
                AppName = healthDto.AppName,
                LastUpdated = DateTime.Now
            };
        }
    }
}