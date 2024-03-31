using Events.Internal.Storage.Entities;

namespace Events.Internal.Interafces
{
    public interface IEventsService 
    {
        Task<Event> GetEvent(int id);
        void UploadMembers(int id, IFormFile file);
    }
}