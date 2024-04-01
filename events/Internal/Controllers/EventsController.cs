using Events.Internal.Dto;
using Events.Internal.Interafces;
using Events.Internal.Storage.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Events.Internal.Controllers 
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(int id) 
        {
            Event result;
            try 
            {
                result = await _eventsService.GetEvent(id);
            }
            catch (Exception err) 
            {
                return new StatusCodeResult(500);
            }

            if (result == null) 
            {
                return new NotFoundObjectResult("This evnet not found");
            }

            return new OkObjectResult(result);
        }

        [HttpPost("{id}/upload")]
        public async Task<IActionResult> UploadUsers(int id, [FromForm] UploadMembersDto dto) 
        {
            try 
            {
                var code = await _eventsService.UploadMembers(id, dto);

                if (code == 0) 
                    return new OkResult();

                else if (code == 404)
                    return new NotFoundResult();

                else if (code == 403)
                    return new ForbidResult();
                else
                    return new StatusCodeResult(500);
            }
            catch (Exception ex) 
            {
                return new StatusCodeResult(500);
            }
        }

        [HttpPatch("{id}/join")]
        public async Task<IActionResult> JoinToEvent(int id, [FromBody] JoinToEventDto dto) 
        {
            try 
            {
                var code = await _eventsService.JoinToEvent(id, dto);

                if (code == 0)
                    return new OkResult();

                else if (code == 404)
                    return new NotFoundResult();
            }
            catch (Exception ex) 
            {
                return new StatusCodeResult(500); 
            }

            return new OkResult();
        }
    }
}