using Events.Internal.Interafces;
using Events.Internal.Storage.Data;
using Events.Internal.Storage.Entities;
using Microsoft.EntityFrameworkCore;

namespace Events.Internal.Storage.Repositories 
{
    public class EventsRepository : IEventsRepository
    {
        private readonly DatabaseContext _context;

        public EventsRepository(DatabaseContext context) 
        {
            _context = context;
        }

        public async void AddEvent(Event toCreate)
        {
            _context.AddAsync(toCreate);
            _context.SaveChangesAsync();
        }

        public async Task<Event> GetEvent(int id)
        {
            Event ev = await _context.events
                .Where(e => e.Id == id)
                .FirstOrDefaultAsync();
                
            return ev;
        }
    }
}