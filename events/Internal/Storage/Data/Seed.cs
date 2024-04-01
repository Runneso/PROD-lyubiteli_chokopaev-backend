using Events.Internal.Storage.Entities;

namespace Events.Internal.Storage.Data 
{
    public static class Seed 
    {
        public static async void Start(DatabaseContext context) 
        {
            Event ev = new Event
            {
                Name = "Олимпиада PROD",
                Description = "Поздравляем с прохождением на заключительный этап! \nСкорее находи команду!",
                StartAt = "2024-03-30 17:30",
                EndAt = "2024-04-04 16:00"
            };

            Event candidate = context.events
                    .Where(e => e.Name == ev.Name)
                    .FirstOrDefault();

            if (candidate == null) 
            {
                context.Add(ev);
            }

            Organizer organizer = new Organizer
            {
                OrgId = 1,
                EventId = 1
            };

            Organizer candidate1 = context.organizers
                .Where(o => o.Id == 1)
                .FirstOrDefault();

            if (candidate1 == null) 
            {
                context.Add(organizer);
            }

            Template template = new Template
            {
                MinLen = 3,
                MaxLen = 5,
                Required = "backend",
                EventId = 1
            };

            Template candidate2 = context.templates
                .Where(t => t.EventId == 1)
                .FirstOrDefault();

            if (candidate2 == null) 
            {
                context.Add(template);
            }

            context.SaveChanges();
        }
    }
}