using Events.Internal.Dto;
using Events.Internal.Storage.Entities;
using Microsoft.AspNetCore.Identity;

namespace Events.Internal.Interafces 
{
    public interface IEventsRepository 
    {
        public Task<List<Event>> GetEvents(int limit, int offset);
        public Task AddEvent(Event toCreate);
        public Event GetEvent(int id);
        public Task<Event> GetEventAsync(int id);
        public Task<Event> GetEventByName(string name);
        public Task UpdateEvent(Event ev);
    }
}