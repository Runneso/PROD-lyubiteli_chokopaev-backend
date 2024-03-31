using System.Diagnostics;
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
    }
}