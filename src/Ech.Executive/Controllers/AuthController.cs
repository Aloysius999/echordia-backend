using Ech.Abstractions.Exceptions;
using Ech.Executive.Authentication.Model;
using Ech.Executive.Authentication.Services;
using Ech.WebApi.Controller;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ech.Executive.Controllers
{
    [Route("api/v1/[controller]")] //    /api/Users
    [ApiController]
    public class AuthController : BaseController<AuthController>
    {
        private IAuthService _authService;

        public AuthController(IConfiguration config, ILogger<AuthController> logger, IAuthService authService)
            : base(config, logger)
        {
            _authService = authService;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(AuthenticateRequest model)
        {
            try
            {
                var response = await _authService.Authenticate(model);

                if (response == null)
                    return BadRequest(new { message = "Username or password is incorrect" });

                return Ok(response);
            }
            catch(NotFoundException ex)
            {
                Logger.LogError(ex.Message);
                return StatusCode(HttpStatusCode.NotFound, ex.Message);
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(HttpStatusCode.Forbidden, ex.Message);
            }
            catch (ValidationErrorException ex)
            {
                return StatusCode(HttpStatusCode.Unauthorized, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
