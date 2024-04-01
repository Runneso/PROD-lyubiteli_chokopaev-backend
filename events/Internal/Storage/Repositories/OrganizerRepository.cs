using Aspose.Cells;
using Events.Internal.Interafces;
using Events.Internal.Storage.Data;
using Events.Internal.Storage.Entities;
using Microsoft.EntityFrameworkCore;

namespace Events.Internal.Storage.Repositories 
{
    public class OrganizerRepository : IOrganizerRepositoy
    {
        private readonly DatabaseContext _context;

        public OrganizerRepository(DatabaseContext context) 
        {
            _context = context;
        }

        public async Task AddOrganizer(Organizer organizer)
        {
            await _context.AddAsync(organizer);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Organizer>> GetOrganizers(int eventId)
        {
            List<Organizer> organizers = await _context.organizers
                .Where(o => o.EventId == eventId)
                .ToListAsync();

            return organizers;
        }
    }
}