using System.Collections.Generic;
using System.Linq;
using Archimedes.Library.Message.Dto;
using Archimedes.Service.Health.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Archimedes.Service.Health
{
    public class HealthDataStore : IHealthDataStore
    {
        private readonly List<HealthMonitorDto> _responses = new List<HealthMonitorDto>();
        private readonly IHubContext<HealthHub> _context;

        public HealthDataStore(IHubContext<HealthHub> context)
        {
            _context = context;
        }

        public void Add(HealthMonitorDto response)
        {
            _responses.Add(response);
            _context.Clients.All.SendAsync("Add", response);
        }

        public void Delete(HealthMonitorDto response)
        {
            _responses.Remove(response);
            _context.Clients.All.SendAsync("Delete", response);
        }

        public void Update(HealthMonitorDto response)
        {
            if (!_responses.Exists(a => a.Url == response.Url))
            {
                Add(response);
                return;
            }

            foreach (var health in _responses.Where(healthMonitorDto => healthMonitorDto.Url == response.Url))
            {
                health.StatusMessage = response.StatusMessage;
                health.LastUpdated = response.LastUpdated;
                health.AppName = response.AppName;
                health.LastActive = response.LastActive;
                health.Status = response.Status;
                health.Version = response.Version;
                health.LastActiveVersion = response.LastActiveVersion;

                _context.Clients.All.SendAsync("Update", health);
                return;
            }
        }

        public List<HealthMonitorDto> Get()
        {
            return _responses;
        }
    }
}