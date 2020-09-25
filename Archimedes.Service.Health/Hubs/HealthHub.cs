using System.Threading.Tasks;
using Archimedes.Library.Message.Dto;
using Microsoft.AspNetCore.SignalR;

namespace Archimedes.Service.Health.Hubs
{
    public class HealthHub : Hub<IHealthHub>
    {
        public async Task Add(HealthMonitorDto value)
        {
            await Clients.All.Add(value);
        }

        public async Task Delete(HealthMonitorDto value)
        {
            await Clients.All.Delete(value);
        }

        public async Task Update(HealthMonitorDto value)
        {
            await Clients.All.Update(value);
        }
    }
}