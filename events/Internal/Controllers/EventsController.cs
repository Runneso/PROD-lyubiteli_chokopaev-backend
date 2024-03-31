using System.Diagnostics;
using Events.Internal.Interafces;
using Events.Internal.Storage.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Events.Internal.Controllers 
{
    [ApiController]
    [Route("events")]
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
        public async Task<IActionResult> UploadUsers(int id, [FromForm(Name = "members")] IFormFile file) 
        {
            try 
            {
                _eventsService.UploadMembers(id, file);
            }
            catch (Exception err) 
            {
                return new StatusCodeResult(500);
            } 
            
            return new OkResult();
        }
    }
}