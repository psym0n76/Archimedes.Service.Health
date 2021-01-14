using System.Threading.Tasks;
using Archimedes.Library.Message.Dto;

namespace Archimedes.Service.Health.Http
{
    public interface IHttpCandleClient
    {
        Task<HealthMonitorDto> GetHealth();
    }
}