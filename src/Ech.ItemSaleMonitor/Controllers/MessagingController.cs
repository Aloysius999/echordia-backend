using Ech.ItemSaleMonitor.Database;
using Ech.ItemSaleMonitor.Services;
using Ech.WebApi;
using Ech.WebApi.API;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using System.Net;

namespace Ech.ItemSaleMonitor.Controllers
{
    [ApiController]
    [Route("api/v1/ism")]
    public class MessagingController : BaseController<MessagingController>
    {
        private readonly IMessagingService __messagingService;
        private readonly IProducingService _queueService;
        private readonly MySQLDbContext _db;

        public MessagingController(IConfiguration config, ILogger<MessagingController> logger, IMessagingService messagingService, IProducingService queueuService, MySQLDbContext db)
            :base (config, logger)
        {
            __messagingService = messagingService;
            _queueService = queueuService;
            _db = db;
        }

        [HttpPost(Name = "PostQuery")]
        public async Task<ActionResult<ApiQueryResponseModel<bool>>> Post([FromBody] ApiRequestModel model)
        {
            await _queueService.SendAsync(
                @object: "hello world",
                exchangeName: "ech.exchange",
                routingKey: "ech.executive");

            return Ok(true);        }
    }
}
