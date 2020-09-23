using System.Net.Http;
using System.Threading.Tasks;
using Archimedes.Library.Message.Dto;

namespace Archimedes.Service.Ui.Http
{
    public interface IHealthResponse
    {
        Task<HealthMonitorDto> Build(HttpResponseMessage response);
    }
}