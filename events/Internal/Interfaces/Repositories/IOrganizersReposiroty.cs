using Events.Internal.Storage.Entities;

namespace Events.Internal.Interafces 
{
    public interface IOrganizerRepositoy 
    {
        Task AddOrganizer(Organizer organizer);
        Task<List<Organizer>> GetOrganizers(int eventId);
    }
}