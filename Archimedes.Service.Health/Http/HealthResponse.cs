using System;
using System.Net.Http;
using System.Threading.Tasks;
using Archimedes.Library.Extensions;
using Archimedes.Library.Message.Dto;
using Microsoft.Extensions.Logging;

namespace Archimedes.Service.Ui.Http
{
    public class HealthResponse : IHealthResponse
    {
        private readonly ILogger<HealthResponse> _logger;

        public HealthResponse(ILogger<HealthResponse> logger)
        {
            _logger = logger;
        }

        public async Task<HealthMonitorDto> Build(HttpResponseMessage response)
        {

            if (!response.IsSuccessStatusCode)
            {

                var errorResponse = await response.Content.ReadAsStringAsync();

                if (response.RequestMessage != null)
                    _logger.LogError(
                        $"GET Failed: {response.ReasonPhrase}  \n\n{errorResponse} \n\n{response.RequestMessage.RequestUri}");
                
                return new HealthMonitorDto()
                {
                    Status = false,
                    StatusMessage = response.ReasonPhrase,
                    Url = response.RequestMessage.RequestUri.ToString()
                };
            }

            var healthDto = await response.Content.ReadAsAsync<HealthMonitorDto>();

            return new HealthMonitorDto()
            {
                Url = response.RequestMessage.RequestUri.ToString(),
                Status = true,
                StatusMessage = response.ReasonPhrase,
                Version = healthDto.Version,
                LastActive = DateTime.Now,
                AppName = healthDto.AppName
            };
        }
    }
}