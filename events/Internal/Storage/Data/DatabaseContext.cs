using Events.Internal.Storage.Entities;
using Microsoft.EntityFrameworkCore;

namespace Events.Internal.Storage.Data 
{
    public class DatabaseContext : DbContext 
    {

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) 
        {
        }

        public DbSet<Event> events { get; set; }
        public DbSet<EventsUsers> eventsUsers { get; set; }
        public DbSet<Organizer> organizers { get; set; }
        public DbSet<Template> templates { get; set; }
    }
}