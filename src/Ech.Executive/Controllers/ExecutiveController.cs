using Ech.Executive.Authentication.Services;
using Ech.Executive.Models.API;
using Ech.Executive.Services;
using Ech.WebApi;
using Microsoft.AspNetCore.Mvc;

namespace Ech.Executive.Controllers
{
    [ApiController]
    [Route("api/v1/exec")]
    public class ExecutiveController : BaseController<ExecutiveController>
    {
        private IExecService _execService;

        public ExecutiveController(IConfiguration config, ILogger<ExecutiveController> logger, IExecService execService)
            :base (config, logger)
        {
            _execService = execService;
        }

        [HttpPost(Name = "PostQuery")]
        public async Task<ActionResult<ApiQueryResponseModel<bool>>> Post([FromBody] ApiRequestModel model)
        {
            return Ok(true);
        }
    }
}
