using Events.Internal.Storage.Entities;

namespace Events.Internal.Storage.Data 
{
    public static class Seed 
    {
        public static void Start(DatabaseContext context) 
        {
            Event ev = new Event
            {
                Name = "Олимпиада PROD",
                Description = "Поздравляем с прохождением на заключительный этап! \nСкорее находи команду!",
                StartAt = "2024-03-30 17:30",
                EndAt = "2024-04-04 16:00",
                OrganizerId = 1
            };

            Event candidate = context.events
                    .Where(e => e.Name == ev.Name)
                    .FirstOrDefault();

            if (candidate == null) 
            {
                context.Add(ev);

                context.SaveChanges();
            }
        }
    }
}