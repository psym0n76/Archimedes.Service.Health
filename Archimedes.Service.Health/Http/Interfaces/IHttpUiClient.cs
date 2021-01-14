using System.Threading.Tasks;
using Archimedes.Library.Message.Dto;

namespace Archimedes.Service.Health.Http
{
    public interface IHttpUiClient
    {
        Task<HealthMonitorDto> GetUiHealth();
    }
}