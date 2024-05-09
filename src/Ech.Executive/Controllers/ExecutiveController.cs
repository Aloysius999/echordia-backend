using Ech.Executive.Models.API;
using Ech.WebApi;
using Microsoft.AspNetCore.Mvc;

namespace Ech.Executive.Controllers
{
    [ApiController]
    [Route("api/v1/exec")]
    public class ExecutiveController : BaseController<ExecutiveController>
    {
        public ExecutiveController(IConfiguration config, ILogger<ExecutiveController> logger)
            :base (config, logger)
        {
        }

        [HttpPost(Name = "PostQuery")]
        public async Task<ActionResult<ApiQueryResponseModel<bool>>> Post([FromBody] ApiRequestModel model)
        {
            return Ok(true);
        }
    }
}
