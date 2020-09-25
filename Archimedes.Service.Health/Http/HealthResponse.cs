using System;
using System.Net.Http;
using System.Threading.Tasks;
using Archimedes.Library.Extensions;
using Archimedes.Library.Message.Dto;

namespace Archimedes.Service.Ui.Http
{
    public class HealthResponse : IHealthResponse
    {
        public async Task<HealthMonitorDto> Build(HttpResponseMessage response)
        {

            var health = new HealthMonitorDto()
            {
                Url = response.RequestMessage.RequestUri.ToString(),
                LastUpdated = DateTime.Now
            };

            if (!response.IsSuccessStatusCode)
            {
                health.Status = false;
                health.StatusMessage = response.ReasonPhrase;
                return health;
            }

            var healthDto = await response.Content.ReadAsAsync<HealthMonitorDto>();

            health.Status = true;
            health.StatusMessage = response.ReasonPhrase;
            health.Version = healthDto.Version;
            health.LastActive = DateTime.Now;
            health.AppName = healthDto.AppName;
            return health;
        }
    }
}