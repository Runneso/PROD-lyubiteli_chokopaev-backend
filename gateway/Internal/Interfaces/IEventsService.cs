using Gateway.Internal.Dto;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace Gateway.Internal.Interfaces 
{
    public interface IEventsService 
    {
        public Task<List<EventDto>> GetEvents();
    }
}