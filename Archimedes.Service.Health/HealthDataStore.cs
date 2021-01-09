using System.Collections.Generic;
using System.Linq;
using Archimedes.Library.Message.Dto;
using Archimedes.Service.Health.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Archimedes.Service.Health
{
    public class HealthDataStore : IHealthDataStore
    {
        private readonly List<HealthMonitorDto> _responses = new();
        private readonly IHubContext<HealthHub> _context;
        private readonly ILogger<HealthDataStore> _logger;

        public HealthDataStore(IHubContext<HealthHub> context, ILogger<HealthDataStore> logger)
        {
            _context = context;
            _logger = logger;
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
            _logger.LogInformation($"Received Health UPDATE: {response.AppName} {response.StatusMessage}");

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