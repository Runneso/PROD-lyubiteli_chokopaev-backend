using Events.Internal.Interafces;
using Events.Internal.Storage.Data;
using Events.Internal.Storage.Entities;
using Microsoft.EntityFrameworkCore;

namespace Events.Internal.Storage.Repositories 
{
    public class EventsUsersRepository : IEventsUsersRepository
    {
        private readonly DatabaseContext _context;

        public EventsUsersRepository(DatabaseContext context) 
        {
            _context = context;
        }

        

        public async Task<EventsUsers> GetPair(long userId, int eventId)
        {
            var pair = await _context.eventsUsers
                .Where(p => (p.EventId == eventId) && (p.UserId == userId))
                .FirstOrDefaultAsync();

            return pair;
        }

        public async Task<EventsUsers> GetPairByTg(string tgUsername, int eventId)
        {
            var pair = await _context.eventsUsers
                .Where(p => (p.Tg == tgUsername) && (p.EventId == eventId))
                .FirstOrDefaultAsync();

            return pair;
        }

        public async Task AddPair(EventsUsers pair)
        {
            await _context.AddAsync(pair);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePair(EventsUsers pair) 
        {
            _context.Update(pair);
            await _context.SaveChangesAsync();
        }

        public async Task<List<EventsUsers>> GetPairs(int eventId)
        {
            var pairs = await _context.eventsUsers
                .Where(e => e.EventId == eventId)
                .ToListAsync();

            return pairs;
        }
    }
}