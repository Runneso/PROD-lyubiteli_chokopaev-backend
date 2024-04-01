using Events.Internal.Dto;
using Events.Internal.Storage.Entities;

namespace Events.Internal.Interafces
{
    public interface IEventsService 
    {
        Task<Event> GetEvent(int id);
        Task<int> UploadMembers(int id, UploadMembersDto dto);
        Task<int> JoinToEvent(int eventId, JoinToEventDto dto);
    }
}