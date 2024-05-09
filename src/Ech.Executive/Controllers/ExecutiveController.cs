using Ech.Abstractions.Database;
using Ech.Executive.Models.API;
using Ech.WebApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;
using System.Text.Json.Nodes;

namespace Ech.Executive.Controllers
{
    [ApiController]
    [Route("api/v1/exec")]
    public class ExecutiveController : BaseController<ExecutiveController>
    {
        public ExecutiveController(IConfiguration config, ILogger<ExecutiveController> logger, IRepositoryBase repository)
            :base (config, logger, repository)
        {
        }

        [HttpPost(Name = "PostQuery")]
        public async Task<ActionResult<ApiQueryResponseModel<bool>>> Post([FromBody] ApiRequestModel model)
        {
            return Ok(true);
        }
    }
}
