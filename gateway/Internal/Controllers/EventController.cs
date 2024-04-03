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

        [HttpGet]
        public async Task<IActionResult> GetEvents() 
        {
            try 
            {
                var result =await _eventsService.GetEvents();

                return new JsonResult(result);
            }
            catch (Exception ex) 
            {
                return new StatusCodeResult(500);
            }
        }
    }
}