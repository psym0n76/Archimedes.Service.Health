using System;
using System.Collections.Generic;
using Archimedes.Library.Logger;
using Archimedes.Library.Message.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Archimedes.Service.Health.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;
        private readonly IHealthDataStore _dataStore;
        private readonly BatchLog _batchLog = new BatchLog();
        private string _logId;

        public HealthController(ILogger<HealthController> logger, IHealthDataStore dataStore)
        {
            _logger = logger;
            _dataStore = dataStore;
        }

        [HttpGet]
        public ActionResult<IEnumerable<HealthMonitorDto>> GetHealthMonitors()
        {
            try
            {
                _logId = _batchLog.Start();
                
                var healthDataStore = _dataStore.Get();

                _logger.LogInformation(_batchLog.Print(_logId,$"Returning {healthDataStore.Count} item(s) from HealthDataStore"));
                return Ok(healthDataStore);

            }
            catch (Exception e)
            {
                _logger.LogError($"Error returned from HealthController", e);
                return BadRequest(e.Message);
            }
        }
    }
}
