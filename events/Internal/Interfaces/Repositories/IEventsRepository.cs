using Events.Internal.Dto;
using Events.Internal.Storage.Entities;
using Microsoft.AspNetCore.Identity;

namespace Events.Internal.Interafces 
{
    public interface IEventsRepository 
    {
        public void AddEvent(Event toCreate);
        public Event GetEvent(int id);
        public Task<Event> GetEventAsync(int id);
        public Task<Event> GetEventByName(string name);
    }
}