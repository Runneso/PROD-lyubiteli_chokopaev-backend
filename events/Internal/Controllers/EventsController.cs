using System.Diagnostics;
using Events.Internal.Dto;
using Events.Internal.Interafces;
using Events.Internal.Storage.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
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

        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] int? limit, [FromQuery] int? offset) 
        {
            try 
            {
                var result = await _eventsService.GetEvents(limit, offset);

                return new JsonResult(result);
            }
            catch (Exception ex) 
            {
                return new StatusCodeResult(500);
            }
        }

        [HttpGet("{id}/users")]
        public async Task<IActionResult> GetUsersOfEvent(int id) 
        {
            try 
            {
                var result = await _eventsService.GetUsersByEvent(id);

                return new JsonResult(result);
            }
            catch (Exception ex) 
            {
                if (ex.Message == "404")
                    return new NotFoundResult();
                return new StatusCodeResult(500);
                
            }
        }

        [HttpPatch("{id}/addRes")]
        public async Task<IActionResult> UploadResults(int id, [FromBody] UploadResultsDto dto) 
        {
            try 
            {
                var code = await _eventsService.UploadResults(id, dto);

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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(int id) 
        {
            EventDto result;
            try 
            {
                result = await _eventsService.GetEvent(id);
            }
            catch (Exception err) 
            {
                return new StatusCodeResult(500);
            }

            if (result.Event == null) 
            {
                return new NotFoundObjectResult("This evnet not found");
            }

            return new JsonResult(result);
        }

        [HttpPost("{id}/addorg")]
        public async Task<IActionResult> AddOrganizer(int id, [FromBody] AddOganizerDto dto) 
        {
            try 
            {
                var code = await _eventsService.AddOrganizer(id, dto);

                if (code == 0)
                    return new OkResult();
                else if (code == 404)
                    return new NotFoundResult();
                else if (code == 403)
                    return new StatusCodeResult(403);
                else
                    return new StatusCodeResult(500);
            }
            catch (Exception ex) 
            {
                return new StatusCodeResult(500);
            }
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
                    return new StatusCodeResult(403);
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

        [HttpPost("{id}/addtemp")]
        public async Task<IActionResult> AddTemplate(int id, [FromBody] CreateTemplateDto dto) 
        {
            try 
            {
                var code = await _eventsService.CreateTemplate(id, dto);

                if (code == 0)
                    return new OkResult();
                else if (code == 404)
                    return new NotFoundResult();
                else if (code == 403)
                    return new StatusCodeResult(403);
                else
                    return new StatusCodeResult(500); 
            }
            catch (Exception ex) 
            {
                return new StatusCodeResult(500); 
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto dto) 
        {
            try 
            {
                var code = await _eventsService.CreateEvent(dto);

                if (code == 0)
                    return new OkResult();
                else if (code == 409)
                    return new ConflictResult();
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