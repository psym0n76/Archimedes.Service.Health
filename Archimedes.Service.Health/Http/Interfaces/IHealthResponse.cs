using System.Net.Http;
using System.Threading.Tasks;
using Archimedes.Library.Message.Dto;

namespace Archimedes.Service.Health.Http
{
    public interface IHealthResponse
    {
        Task<HealthMonitorDto> Build(HttpResponseMessage response);
    }
}