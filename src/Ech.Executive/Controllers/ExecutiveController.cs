using Ech.Executive.Database;
using Ech.Executive.Services;
using Ech.Schema.Executive;
using Ech.WebApi;
using Ech.WebApi.API;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using System.Net;

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
            User user = _db.Users.FirstOrDefault<User>(item => item.id == model.userId);

            if (user != null)
            {
                await _queueService.SendAsync(
                    @object: user,
                    exchangeName: "ech.exchange",
                    routingKey: "ech.serv.itemsalemonitor");

                return Ok(true);
            }

            return StatusCode(HttpStatusCode.BadRequest, "User not found");
        }
    }
}
