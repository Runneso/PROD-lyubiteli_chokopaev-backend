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
        
        public EventsService(
            IEventsRepository eventsRepository,
            IMailSerivice mailSerivice,
            IEventsUsersRepository eventsUsersRepository,
            IOrganizerRepositoy organizerRepositoy
        ) 
        {
            _eventsRepository = eventsRepository;
            _mailSerivice = mailSerivice;
            _pairRepository = eventsUsersRepository;
            _organizerRepositoy = organizerRepositoy;
        }

        public async Task<Event> GetEvent(int id) 
        {
            var result = await _eventsRepository.GetEventAsync(id);
            
            return result;
        }

        public async Task<int> UploadMembers(int id, UploadMembersDto dto)
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
                if (organizer.Id == dto.OrganizerId) 
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
                Console.WriteLine(email);
                if (email != null) 
                {
                    string tgUsername =  worksheet.Cells[i, 5].Value.ToString();
                    await ProcessMember(tgUsername, email.ToString(), ev);
                }
                else 
                {
                    continue;
                }
                Console.WriteLine(i);
            }

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

    }
}