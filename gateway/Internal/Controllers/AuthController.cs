using System.Diagnostics.CodeAnalysis;
using Gateway.Internal.Dto;
using Gateway.Internal.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Internal.Controllers 
{
    [ApiController]
    [Route("api/v1/users/auth")]
    public class AuthController : Controller 
    {
        private readonly IUsersService _usersService;

        public AuthController(IUsersService usersService) 
        {
            _usersService = usersService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoignDto dto) 
        {
            try 
            {
                var result = await _usersService.Login(dto);

                return new JsonResult(result);
            }
            catch (Exception ex) 
            {
                if (ex.Message == "401")
                    return new UnauthorizedResult();
                else if (ex.Message == "404")
                    return new NotFoundResult();
                else if (ex.Message == "422")
                    return new StatusCodeResult(422);
                else
                    return new StatusCodeResult(500);  
            }
        }

        [HttpPost("registration")]
        public async Task<IActionResult> UsersRegistration([FromForm] CreateUserDto dto) 
        {
            try 
            {
                var result = await _usersService.CreateUser(dto);
                Response.StatusCode = 201;
                return new JsonResult(result);
            }
            catch (Exception ex) 
            {
                if (ex.Message == "409")
                    return new ConflictResult();
                else if (ex.Message == "422")
                    return new StatusCodeResult(422);
                else if (ex.Message == "404")
                    return new NotFoundResult(); 
                return new StatusCodeResult(500);
            }
        }
    }
}