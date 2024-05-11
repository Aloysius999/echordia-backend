using Ech.Executive.Authentication.Services;
using Ech.Executive.Models.API;
using Ech.Executive.Services;
using Ech.WebApi;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;

namespace Ech.Executive.Controllers
{
    [ApiController]
    [Route("api/v1/exec")]
    public class ExecutiveController : BaseController<ExecutiveController>
    {
        private readonly IExecService _execService;
        private readonly IProducingService _queueService;

        public ExecutiveController(IConfiguration config, ILogger<ExecutiveController> logger, IExecService execService, IProducingService queueuService)
            :base (config, logger)
        {
            _execService = execService;
            _queueService = queueuService;
        }

        [HttpPost(Name = "PostQuery")]
        public async Task<ActionResult<ApiQueryResponseModel<bool>>> Post([FromBody] ApiRequestModel model)
        {
            await _queueService.SendAsync(
                @object: model,
                exchangeName: "exchange.name",
                routingKey: "routing.key");

            return Ok(true);
        }
    }
}
