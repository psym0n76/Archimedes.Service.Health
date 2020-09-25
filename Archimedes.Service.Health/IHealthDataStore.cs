using System.Collections.Generic;
using Archimedes.Library.Message.Dto;

namespace Archimedes.Service.Health
{
    public interface IHealthDataStore
    {
        void Add(HealthMonitorDto response);
        void Delete(HealthMonitorDto response);
        void Update(HealthMonitorDto response);

        List<HealthMonitorDto> Get();
    }
}