using Gateway.Internal.Dto;
using Gateway.Internal.Interfaces;
using Gateway.Internal.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Internal.Controllers
{
    [ApiController]
    [Route("api/v1/events")]
    public class EventsController : Controller 
    {
        private readonly IEventsService _eventsService;

        public EventsController(IEventsService eventsService) 
        {
            _eventsService = eventsService;
        }

        [HttpPost("{id}/upload/users")]
        public async Task<IActionResult> UploadUsers(int id, [FromForm] UploadUsers dto, [FromHeader(Name = "Authorization")] string token) 
        {
            try 
            {
                await _eventsService.UploadUsers(id, dto, token);
                return new OkResult();
            }
            catch (Exception ex) 
            {
                return new StatusCodeResult(500);
            }
        }

        [HttpGet("{id}/statistic")]
        public async Task<IActionResult> GetStatistic(int id, [FromHeader(Name = "Authorization")] string token) 
        {
            try 
            {
                var res = await _eventsService.GetStat(id);

                return new JsonResult(res);
            }
            catch (Exception ex) 
            {
                return new StatusCodeResult(500);
            }
        }

    }
}