using Events.Internal.Interafces;
using Events.Internal.Storage.Entities;
using Aspose.Cells;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Expressions;
using Events.Internal.Dto;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using System.Runtime.ConstrainedExecution;

namespace Events.Internal.Services 
{
    public class EventsService : IEventsService 
    {

        private readonly IEventsRepository _eventsRepository;
        private readonly IMailSerivice _mailSerivice;
        private readonly IEventsUsersRepository _pairRepository;
        private readonly IOrganizerRepositoy _organizerRepositoy;
        private readonly ITemplatesRepository _templatesRepository;
        
        public EventsService(
            IEventsRepository eventsRepository,
            IMailSerivice mailSerivice,
            IEventsUsersRepository eventsUsersRepository,
            IOrganizerRepositoy organizerRepositoy,
            ITemplatesRepository templatesRepository
        ) 
        {
            _eventsRepository = eventsRepository;
            _mailSerivice = mailSerivice;
            _pairRepository = eventsUsersRepository;
            _organizerRepositoy = organizerRepositoy;
            _templatesRepository = templatesRepository;
        }

        public async Task<EventDto> GetEvent(int id) 
        {
            var ev = await _eventsRepository.GetEventAsync(id);

            var temp = await _templatesRepository.GetTemplate(id);

            var result = new EventDto 
            {
                Event = ev
            };

            if (temp != null) 
            {
                result.TeamsTemplate = temp;
            }
            
            return result;
        }

        public async Task<int> UploadResults(int id, UploadResultsDto dto) 
        {
            var ev = await _eventsRepository.GetEventAsync(id);

            if (ev == null) 
                return 404;

            ev.ResultsPath = dto.ResultsPath;

            await _eventsRepository.UpdateEvent(ev);

            return 0;
        }

        public async Task<int> UploadMembers(int id,  UploadMembersDto dto)
        {
            var ev = _eventsRepository.GetEvent(id);

            if (ev == null) 
            {
                return 404;
            }   

            var organizers = await _organizerRepositoy.GetOrganizers(id);

            bool flag = false;

            foreach (var organizer in organizers) 
            {
                if (organizer.OrgId == dto.OrganizerId) 
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
                return 403; 

            string path = Path.Combine("Files", $"{Guid.NewGuid().ToString()}.xlsx");
            
            using (FileStream stream = File.Create(path)) 
            {
                dto.file.CopyTo(stream);
            }

            Workbook workbook = new Workbook(path);
            Worksheet worksheet = workbook.Worksheets[0];

            for (int i = 1; i < worksheet.Cells.Rows.Count; i++) 
            {
                object email = worksheet.Cells[i, 4].Value;

                if (email != null) 
                {
                    string tgUsername =  worksheet.Cells[i, 5].Value.ToString();
                    await ProcessMember(tgUsername, email.ToString(), ev);
                }
                else 
                {
                    continue;
                }
            }

            ev.MembersListPath = dto.MembersPath;

            await _eventsRepository.UpdateEvent(ev);

            File.Delete(path);

            return 0; 
        }

        public async Task ProcessMember(string tgUsername, string email, Event ev) 
        {
            var client = new HttpClient();
            string url = Environment.GetEnvironmentVariable("USERS_SERVICE_URL");
            var response = await client.GetAsync($"{url}/api/v1/users/get_user_by_tg/{tgUsername}");

            var pair = new EventsUsers() 
            {
                EventId = ev.Id,
                IsJoin = false
            };

            if (response.StatusCode.ToString() == "OK") 
            {
                var user = await response.Content.ReadFromJsonAsync<UserDto>();            

                pair.UserId = user.id;

                var candidate = await _pairRepository.GetPair(user.id, ev.Id);

                if (candidate == null) 
                {
                    await _pairRepository.AddPair(pair);

                    await _mailSerivice.SendInviteToEvent(email, ev.Name); 
                }
            }
            else 
            {
                pair.Tg = tgUsername;
                
                var candidate = await _pairRepository.GetPairByTg(tgUsername, ev.Id);

                if (candidate == null) 
                {
                    try 
                    {
                        await _pairRepository.AddPair(pair);

                        await _mailSerivice.SendInviteToApp(email, ev.Name);
                    }
                    catch (Exception ex) {}
                 }
            }
        }

        public async Task<int> JoinToEvent(int eventId, JoinToEventDto dto) 
        {
            var pair = await _pairRepository.GetPair(dto.UserId, eventId);

            if (pair == null && dto.TgUsername != null) 
            {
                pair = await _pairRepository.GetPairByTg(dto.TgUsername, eventId);

                if (pair == null) 
                    return 404;
            }
            else if (pair == null && dto.TgUsername == null) 
                return 404;

            pair.IsJoin = true;

            if (pair.UserId == null) 
                pair.UserId = dto.UserId;

            await _pairRepository.UpdatePair(pair);

            return 0;
        }

        public async Task<int> AddOrganizer(int eventId, AddOganizerDto dto)
        {
            var ev = await _eventsRepository.GetEventAsync(eventId);

            if (ev == null)
                return 404;

            var organizers = await _organizerRepositoy.GetOrganizers(eventId);

            bool flag = false;

            foreach (var organizer in organizers) 
            {
                if (organizer.OrgId == dto.OrganizerId) 
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
                return 403; 

            var org = new Organizer
            {
                OrgId = dto.UserId,
                EventId = eventId
            };

            await _organizerRepositoy.AddOrganizer(org);

            return 0;
        }

        public async Task<int> CreateTemplate(int eventId, CreateTemplateDto dto)
        {
            var ev = await _eventsRepository.GetEventAsync(eventId);

            if (ev == null)
                return 404;

            var organizers = await _organizerRepositoy.GetOrganizers(eventId);

            bool flag = false;

            foreach (var organizer in organizers) 
            {
                if (organizer.OrgId == dto.OrgId) 
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
                return 403; 

            var candidate = await _templatesRepository.GetTemplate(eventId);

            if (candidate != null) 
            {
                await _templatesRepository.RmTemplate(candidate);
            }

            var template = new Template
                {
                    EventId = ev.Id,
                    MinLen = dto.MinLen,
                    MaxLen = dto.MaxLen,
                    Required = string.Join(";", dto.Required)
                };

                await _templatesRepository.AddTemplate(template);

            return 0;
        }

        public async Task<int> CreateEvent(CreateEventDto dto)
        {
            var candidate = await _eventsRepository.GetEventByName(dto.Name);

            if (candidate != null)
                return 409; 

            var ev = new Event 
            {
                Name = dto.Name,
                Description = dto.Description,
                StartAt = dto.StartAt,
                EndAt = dto.EndAt
            };

            if (dto.ImagePath != null)
                ev.ImagePath = dto.ImagePath;

            await _eventsRepository.AddEvent(ev);

            var org = new Organizer 
            {
                EventId = ev.Id,
                OrgId = dto.OrgId
            };

            await _organizerRepositoy.AddOrganizer(org);

            if (dto.MaxLenTeam != null && dto.MinLenTeam != null && dto.Required != null) 
            {
                var temp = new Template
                {
                    EventId = ev.Id,
                    MaxLen = (int)dto.MaxLenTeam,
                    MinLen = (int)dto.MinLenTeam,
                    Required = string.Join(";", dto.Required)
                };

                await _templatesRepository.AddTemplate(temp);
            }

            return 0; 
        }

        public async Task<List<Event>> GetEvents(int? limit, int? offset) 
        {
            limit ??= 5;
            offset ??= 0;

            var result = await _eventsRepository.GetEvents((int)limit, (int)offset);

            return result;
        }

        public async Task<List<int>> GetUsersByEvent(int eventId)
        {
            var toCheck = await _eventsRepository.GetEventAsync(eventId);

            if (toCheck == null)
                throw new Exception("404");

            var pairs = await _pairRepository.GetPairs(eventId);
            var result = new List<int>();
            foreach (var p in pairs) 
            {
                if (p.IsJoin) 
                {
                    result.Add((int)p.UserId);
                }
            }
            return result;
        }
    }
}