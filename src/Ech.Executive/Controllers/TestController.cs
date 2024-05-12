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
    [Route("api/v1/test")]
    public class TestController : BaseController<TestController>
    {
        private readonly ITestService _testService;
        private readonly IProducingService _queueService;

        public TestController(IConfiguration config, ILogger<TestController> logger, ITestService testService, IProducingService queueuService)
            :base (config, logger)
        {
            _testService = testService;
            _queueService = queueuService;
        }

        [HttpPost(Name = "PostTestQuery")]
        public async Task<ActionResult<ApiQueryResponseModel<bool>>> Post([FromBody] ApiRequestModel model)
        {
            await _queueService.SendAsync(
                @object: model,
                exchangeName: "ech.exchange",
                routingKey: "ech.test.service");

            return Ok(true);
        }
    }
}
