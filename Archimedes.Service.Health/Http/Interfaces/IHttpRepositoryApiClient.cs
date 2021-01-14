using System.Threading.Tasks;
using Archimedes.Library.Message.Dto;

namespace Archimedes.Service.Health.Http
{
    public interface IHttpRepositoryApiClient
    {
        Task<HealthMonitorDto> GetRepositoryApiHealth();
    }
}