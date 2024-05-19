using Ech.Executive.Database;
using Ech.Executive.Services;
using Ech.Queries.IntraService;
using Ech.Schema.Executive;
using Ech.Schema.IntraService;
using Ech.WebApi;
using Ech.WebApi.API;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using System.Net;
using QueryBase = Ech.Queries.IntraService.QueryBase;

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

        //-----------------------------------------
        // POST
        //-----------------------------------------
        [HttpPost]
        public async Task<ActionResult<ApiQueryResponseModel<bool>>> Post([FromBody] ApiRequestModel model)
        {
            try
            {
                switch(model.queryType)
                {
                    case QueryType.SaleControl:
                        return await PostSaleControl(model);
                    default:
                        return StatusCode(HttpStatusCode.NotImplemented, string.Format("Invalid queryType {0}", model.queryType));
                }
            }
            catch(Exception ex)
            {
                return StatusCode(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private async Task<ActionResult<ApiQueryResponseModel<bool>>> PostSaleControl(ApiRequestModel model)
        {
            SaleControl saleControl = _db.SaleControls.FirstOrDefault<SaleControl>(item => (item.itemId == model.itemId
                                                                                            && item.userId == model.userId));

            if (saleControl != null)
            {
                await _queueService.SendAsync(
                    @object: new ItemSaleMonitorQuery(saleControl, QueryBase.QueryType.New),
                    exchangeName: "ech.exchange",
                    routingKey: "ech.serv.itemsalemonitor");

                return Ok(true);
            }

            return StatusCode(HttpStatusCode.BadRequest, "User not found");
        }

        //-----------------------------------------
        // PUT
        //-----------------------------------------
        [HttpPut]
        public async Task<ActionResult<ApiQueryResponseModel<bool>>> Put([FromBody] ApiRequestModel model)
        {
            try
            {
                switch (model.queryType)
                {
                    case QueryType.SaleControl:
                        return await PutSaleControl(model);
                    default:
                        return StatusCode(HttpStatusCode.NotImplemented, string.Format("Invalid queryType {0}", model.queryType));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private async Task<ActionResult<ApiQueryResponseModel<bool>>> PutSaleControl(ApiRequestModel model)
        {
            SaleControl saleControl = _db.SaleControls.FirstOrDefault<SaleControl>(item => (item.itemId == model.itemId
                                                                                            && item.userId == model.userId));

            if (saleControl != null)
            {
                await _queueService.SendAsync(
                    @object: new ItemSaleMonitorQuery(saleControl, QueryBase.QueryType.Update),
                    exchangeName: "ech.exchange",
                    routingKey: "ech.serv.itemsalemonitor");

                return Ok(true);
            }

            return StatusCode(HttpStatusCode.BadRequest, "User not found");
        }
    }
}
