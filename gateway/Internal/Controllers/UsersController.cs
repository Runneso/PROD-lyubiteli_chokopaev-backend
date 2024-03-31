using Gateway.Internal.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Internal.Controllers 
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : Controller 
    {

        private IUsersService _usersService;

        public UsersController(IUsersService usersService) 
        {
            _usersService = usersService;
        }

        [HttpGet("myprofile")]
        public IActionResult GetMyProfile() 
        {
            return new OkResult();
        }

        [HttpGet("profile/{id}")]
        public IActionResult GetUserProfile() 
        {
            return new OkResult();
        }

        [HttpPost("auth/login")]
        public IActionResult Login() 
        {
            return new OkResult();
        }

        [HttpPost("auth/logout")]
        public IActionResult Logout() 
        {
            return new OkResult();
        }

        [HttpPost("registration")]
        public IActionResult UsersRegistration() 
        {
            return new OkResult();
        }

    }
}