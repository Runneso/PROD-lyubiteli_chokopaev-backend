using Events.Internal.Dto;
using Events.Internal.Storage.Entities;

namespace Events.Internal.Interafces
{
    public interface IEventsService 
    {
        Task<EventDto> GetEvent(int id);
        Task<int> CreateEvent(CreateEventDto dto);
        Task<int> UploadMembers(int id,  UploadMembersDto dto);
        Task<int> JoinToEvent(int eventId, JoinToEventDto dto);
        Task<int> AddOrganizer(int eventId, AddOganizerDto dto);
        Task<int> CreateTemplate(int eventId, CreateTemplateDto dto);
    }
}