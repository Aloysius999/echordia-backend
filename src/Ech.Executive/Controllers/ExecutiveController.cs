using Ech.Executive.Authentication.Services;
using Ech.Executive.Models.API;
using Ech.Executive.Services;
using Ech.WebApi;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using Ech.Executive.Database;
using Ech.Executive.Authentication.Model;
using Ech.Executive.Schema;

namespace Ech.Executive.Controllers
{
    [ApiController]
    [Route("api/v1/exec")]
    public class ExecutiveController : BaseController<ExecutiveController>
    {
        private readonly IExecService _execService;
        private readonly IProducingService _queueService;
        private readonly MySQLDbContext _db;

        public ExecutiveController(IConfiguration config, ILogger<ExecutiveController> logger, IExecService execService, IProducingService queueuService, MySQLDbContext db)
            :base (config, logger)
        {
            _execService = execService;
            _queueService = queueuService;
            _db = db;
        }

        [HttpPost(Name = "PostQuery")]
        public async Task<ActionResult<ApiQueryResponseModel<bool>>> Post([FromBody] ApiRequestModel model)
        {
            //await _queueService.SendAsync(
            //    @object: model,
            //    exchangeName: "ech.exchange",
            //    routingKey: "ech.item.sale.monitor");

            User user = _db.Users.FirstOrDefault<User>(item => item.id == model.userId);

            return Ok(true);
        }
    }
}
