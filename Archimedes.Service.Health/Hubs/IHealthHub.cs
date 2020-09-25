using System.Threading.Tasks;
using Archimedes.Library.Message.Dto;

namespace Archimedes.Service.Health.Hubs
{
    public interface IHealthHub
    {
        Task Add(HealthMonitorDto value);
        Task Delete(HealthMonitorDto value);
        Task Update(HealthMonitorDto value);
    }
}