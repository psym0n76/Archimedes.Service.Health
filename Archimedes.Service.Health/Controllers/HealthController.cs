using System;
using System.Collections.Generic;
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
                var healthDataStore = _dataStore.Get();
                return Ok(healthDataStore);

            }
            catch (Exception e)
            {
                _logger.LogError(e.StackTrace);
                return BadRequest();
            }
        }
    }
}
