using Events.Internal.Interafces;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;

namespace Events.Internal.Controllers 
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

        [HttpPost("distribute/random/{id}")]
        public async Task<IActionResult> DistributeRandom(int id) 
        {
            try 
            {
                var code = await _teamsService.DistributeRandom(id);

                if (code == 0)
                    return new OkResult();
                else if (code == 404)
                    return new NotFoundResult();
                else
                    return new StatusCodeResult(500); 
            }
            catch (Exception ex) 
            {
                return new StatusCodeResult(500);
            }
        }
    }
}