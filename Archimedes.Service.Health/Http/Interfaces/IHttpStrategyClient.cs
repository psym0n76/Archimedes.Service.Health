using System.Threading.Tasks;
using Archimedes.Library.Message.Dto;

namespace Archimedes.Service.Ui.Http
{
    public interface IHttpStrategyClient
    {
        Task<HealthMonitorDto> GetStrategyHealth();
    }
}