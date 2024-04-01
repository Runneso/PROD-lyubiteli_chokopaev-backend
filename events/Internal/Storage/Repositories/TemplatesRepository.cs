using Events.Internal.Interafces;
using Events.Internal.Storage.Data;
using Events.Internal.Storage.Entities;
using Microsoft.EntityFrameworkCore;

namespace Events.Internal.Storage.Repositories 
{
    public class TemplatesRepository : ITemplatesRepository
    {

        private readonly DatabaseContext _context;

        public TemplatesRepository(DatabaseContext context) 
        {
            _context = context;
        }

        public async Task AddTemplate(Template template)
        {
            await _context.AddAsync(template);
            await _context.SaveChangesAsync();
        }

        public async Task<Template> GetTemplate(int eventId)
        {
            var temp = await _context.templates
                .Where(t => t.EventId == eventId)
                .FirstOrDefaultAsync();

            return temp;
        }

        public async Task RmTemplate(Template template)
        {
            _context.Remove(template);
            await _context.SaveChangesAsync();
        }
    }
}