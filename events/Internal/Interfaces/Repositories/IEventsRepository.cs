using Events.Internal.Storage.Entities;
using Microsoft.AspNetCore.Identity;

namespace Events.Internal.Interafces 
{
    public interface IEventsRepository 
    {
        public void AddEvent(Event toCreate);
        public Task<Event> GetEvent(int id);
    }
}