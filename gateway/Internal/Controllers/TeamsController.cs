using System.Diagnostics;
using System.Runtime.CompilerServices;
using Gateway.Internal.Dto;
using Gateway.Internal.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Internal.Controllers 
{
    [ApiController]
    [Route("api/v1/teams")]
    public class TeamsController : Controller
    {
        private readonly ITeamsService _teamsService;

        public TeamsController(ITeamsService teamsService) 
        {
            _teamsService = teamsService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeamById(int id, [FromHeader(Name = "Authorization")] string token) 
        {
            try 
            {
                var result = await _teamsService.GetTeamById(id, token);

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

        [HttpGet("all/{eventId}")]
        public async Task<IActionResult> GetTeamnsByEvenet(int eventId, [FromHeader(Name = "Authorization")] string token) 
        {
            try 
            {
                var result = await _teamsService.GetTeamByEvent(eventId, token);

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

        [HttpPost("create")]
        public async Task<IActionResult> CreateTeam([FromBody] CreateTeamDto dto, [FromHeader(Name = "Authorization")] string token) 
        {
            try
            {
                await _teamsService.CreateTeam(dto, token);

                return new StatusCodeResult(201);
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
                else if (ex.Message == "415")
                    return new StatusCodeResult(415);
                else
                    return new StatusCodeResult(500);
            }
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> DeleteTeam([FromQuery] int team_id, [FromHeader(Name = "Authorization")] string token) 
        {
            try 
            {
                await _teamsService.DeleteTeam(team_id, token);

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
                else if (ex.Message == "415")
                    return new StatusCodeResult(415);
                else
                    return new StatusCodeResult(500);
            }
        }

        [HttpPatch("update")]
        public async Task<IActionResult> UpdateTeam([FromBody] UpdateTeamDto dto, [FromHeader(Name = "Authorization")] string token) 
        {
            try 
            {
                await _teamsService.UpdateTeam(dto, token);

                return new OkResult(); 
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
                else if (ex.Message == "415")
                    return new StatusCodeResult(415);
                else
                    return new StatusCodeResult(500);
            }
        }

        [HttpPost("tags/create")]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagDto dto, [FromHeader(Name = "Authorization")] string token) 
        {
            try 
            {
                await _teamsService.CreateTag(dto, token);

                return new StatusCodeResult(201);
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
                else if (ex.Message == "415")
                    return new StatusCodeResult(415);
                else
                    return new StatusCodeResult(500);
            }
        }

        [HttpDelete("tags/delete")]
        public async Task<IActionResult> DeleteTag([FromQuery] int team_id, [FromQuery] string tag, [FromHeader(Name = "Authorization")] string token) 
        {
            try 
            {
                await _teamsService.DeleteTag(team_id, tag, token);

                return new OkResult();
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
                else if (ex.Message == "415")
                    return new StatusCodeResult(415);
                else
                    return new StatusCodeResult(500);
            }

        }

        [HttpPost("invites/create")]
        public async Task<IActionResult> CreateInvite([FromBody] CreateInviteDto dto, [FromHeader(Name = "Authorization")] string token) 
        {
            try 
            {
                await _teamsService.CreateInvite(dto, token);

                return new StatusCodeResult(201);
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
                else if (ex.Message == "403")
                    return new StatusCodeResult(403);
                else if (ex.Message == "415")
                    return new StatusCodeResult(415);
                else
                    return new StatusCodeResult(500);
            }
        }

        [HttpGet("invites")]
        public async Task<IActionResult> GetInvites([FromBody] GetInviteDto dto, [FromHeader(Name = "Authorization")] string token) 
        {
            try 
            {
                var result = await _teamsService.GetInvites(dto, token);

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
                else if (ex.Message == "403")
                    return new StatusCodeResult(403);
                else if (ex.Message == "415")
                    return new StatusCodeResult(415);
                else
                    return new StatusCodeResult(500);
            }
        }

        [HttpPost("invites/answer")]
        public async Task<IActionResult> AnswerInvite([FromBody] AnswerInviteDto dto, [FromHeader(Name = "Authorization")] string token) 
        {
            try 
            {
                await _teamsService.AnswerInvite(dto, token);

                return new OkResult();
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
                else if (ex.Message == "403")
                    return new StatusCodeResult(403);
                else if (ex.Message == "415")
                    return new StatusCodeResult(415);
                else
                    return new StatusCodeResult(500);
            }
        }

        [HttpGet("all/possible/{eventId}")]
        public async Task<IActionResult> GetPossible(int eventId, [FromQuery] int offset, [FromHeader(Name = "Authorization")] string token) 
        {
            try 
            {
                var res = await _teamsService.GetPossible(eventId, offset, token);

                return new JsonResult(res);
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
                else if (ex.Message == "403")
                    return new StatusCodeResult(403);
                else if (ex.Message == "415")
                    return new StatusCodeResult(415);
                else
                    return new StatusCodeResult(500);
            }
        }

        [HttpGet("my/{id}")]
        public async Task<IActionResult> GetMy(int id, [FromHeader(Name = "Authorization")] string token) 
        {
            try 
            {
                var res = await _teamsService.My(id, token);

                return new JsonResult(res);
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
                else if (ex.Message == "403")
                    return new StatusCodeResult(403);
                else if (ex.Message == "415")
                    return new StatusCodeResult(415);
                else
                    return new StatusCodeResult(500);
            }
        }

        [HttpDelete("from/{team_id}/delete/{userId}")]
        public async Task<IActionResult> RmFrom(int team_id, int userId, [FromHeader(Name = "Authorization")] string token) 
        {
            try 
            {
                await _teamsService.DeleteUser(team_id, userId, token);

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
                else if (ex.Message == "403")
                    return new StatusCodeResult(403);
                else if (ex.Message == "415")
                    return new StatusCodeResult(415);
                else
                    return new StatusCodeResult(500);
            }
        }
    }
}