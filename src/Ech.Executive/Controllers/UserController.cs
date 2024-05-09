using Ech.Executive.Authentication.Helpers;
using Ech.Executive.Authentication.Model;
using Ech.Executive.Authentication.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ech.Executive.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController] // Controller level
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        //[ApiController]  // Action method level
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] bool? isActive = null)
        {
            var heros = await _userService.GetAllUsers(isActive);
            return Ok(heros);
        }

        [HttpGet("{id}")]
        //[Route("{id}")] // /api/OurHero/:id
        public async Task<IActionResult> Get(int id)
        {
            var hero = await _userService.GetUserById(id);
            if (hero == null)
            {
                return NotFound();
            }
            return Ok(hero);
        }
    }
}
