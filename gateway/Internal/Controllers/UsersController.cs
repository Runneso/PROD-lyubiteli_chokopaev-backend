using System.Diagnostics;
using Gateway.Internal.Dto;
using Gateway.Internal.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Internal.Controllers 
{
    [ApiController]
    [Route("api/v1/users")]
    public class UsersController : Controller 
    {

        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService) 
        {
            _usersService = usersService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetMyProfile([FromHeader(Name = "Authorization")] string token) 
        {
            try 
            {
                var result = await _usersService.GetProfile(token);

                return new JsonResult(result);
            }
            catch (Exception ex) 
            {
                if (ex.Message == "401")
                    return new UnauthorizedResult();
                else if (ex.Message == "404")
                    return new NotFoundResult();
                else if (ex.Message == "409")
                    return new ConflictResult();
                else if (ex.Message == "422") 
                    return new StatusCodeResult(422);
                else
                    return new StatusCodeResult(500);
            }
        }

        [HttpGet("profile/{id}")]
        public async  Task<IActionResult> GetUserProfile(int id, [FromHeader(Name = "Authorization")] string token) 
        {
            try 
            {
                var result = await _usersService.GetProfileById(id, token);

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

        [HttpPatch("profile/update")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto dto, [FromHeader(Name = "Authorization")] string token) 
        {
            try 
            {
                var result = await _usersService.UpdateUser(dto, token);

                return new JsonResult(result); 
            }
            catch (Exception ex) 
            {
                if (ex.Message == "401")
                    return new UnauthorizedResult();
                else if (ex.Message == "404")
                    return new NotFoundResult();
                else if (ex.Message == "409")
                    return new ConflictResult();
                else if (ex.Message == "422") 
                    return new StatusCodeResult(422);
                else
                    return new StatusCodeResult(500);
            }
        }

        [HttpDelete("profile/delete")]
        public async Task<IActionResult> DeleteProfile([FromHeader(Name = "Authorization")] string token) 
        {
            try 
            {
                await _usersService.DeleteUser(token);

                return new StatusCodeResult(204);
            }
            catch (Exception ex) 
            {
                if (ex.Message == "401")
                    return new UnauthorizedResult();
                else if (ex.Message == "404")
                    return new NotFoundResult();
                else if (ex.Message == "409")
                    return new ConflictResult();
                else if (ex.Message == "422") 
                    return new StatusCodeResult(422);
                else
                    return new StatusCodeResult(500);
            }
        }

    }
}