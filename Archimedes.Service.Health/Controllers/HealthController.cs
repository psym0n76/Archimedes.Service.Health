using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Archimedes.Library.Message.Dto;
using Archimedes.Service.Ui.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Archimedes.Service.Health.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;
        private readonly IHttpBrokerClient _brokerClient;
        private readonly IHttpCandleClient _candleClient;
        private readonly IHttpRepositoryClient _repositoryClient;
        private readonly IHttpRepositoryApiClient _repositoryApiClient;
        private readonly IHttpUiClient _uiClient;



        public HealthController(ILogger<HealthController> logger, IHttpBrokerClient brokerClient,
            IHttpCandleClient candleClient, IHttpRepositoryClient repositoryClient,
            IHttpRepositoryApiClient repositoryApiClient, IHttpUiClient uiClient)
        {
            _logger = logger;
            _brokerClient = brokerClient;
            _candleClient = candleClient;
            _repositoryClient = repositoryClient;
            _repositoryApiClient = repositoryApiClient;
            _uiClient = uiClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HealthMonitorDto>>> GetHealthMonitors()
        {
            //todo create an in-memory list
            var healthMonitors = new List<HealthMonitorDto>();

            try
            {
                var broker = await _brokerClient.GetBrokerHealth();
                healthMonitors.Add(broker);

                var candle = await _candleClient.GetCandleHealth();
                healthMonitors.Add(candle);

                var repository = await _repositoryClient.GetRepositoryHealth();
                healthMonitors.Add(repository);

                var repositoryApi = await _repositoryApiClient.GetRepositoryApiHealth();
                healthMonitors.Add(repositoryApi);

                var ui = await _uiClient.GetUiHealth();
                healthMonitors.Add(ui);

                return Ok(healthMonitors);

            }
            catch (Exception e)
            {
                _logger.LogError(e.StackTrace);
                return BadRequest();
            }
        }
    }
}
