using System.Diagnostics.Tracing;
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

        public async Task AddEvent(Event toCreate)
        {
            await _context.AddAsync(toCreate);
            await _context.SaveChangesAsync();
        }

        public  Event GetEvent(int id)
        {
            Event ev =  _context.events
                .Where(e => e.Id == id)
                .FirstOrDefault();
                
            return ev;
        }

        public async Task<Event> GetEventAsync(int id) 
        {
            Event ev = await _context.events
                .Where(e => e.Id == id)
                .FirstOrDefaultAsync();
                
            return ev;
        }

        public async Task<Event> GetEventByName(string name)
        {
            var ev = await _context.events
                .Where(e => e.Name == name)
                .FirstOrDefaultAsync(); 
            return ev;
        }

        public async Task<List<Event>> GetEvents(int limit, int offset)
        {
            var events = await _context.events
                .OrderBy(e => e.Id)
                .Skip(offset).Take(limit)
                .ToListAsync();

            return events;
        }

        public async Task UpdateEvent(Event ev)
        {
            _context.Update(ev);
            await _context.SaveChangesAsync();
        }
    }
}