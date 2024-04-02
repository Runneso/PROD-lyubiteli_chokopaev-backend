using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Aspose.Cells.Vba;
using Events.Internal.Dto;
using Events.Internal.Interafces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.WebUtilities;

namespace Events.Internal.Services 
{
    public class TeamsService : ITeamsService
    {
        private readonly IEventsUsersRepository _pairsRepository;
        private readonly IEventsRepository _eventsRepository;
        private readonly ITemplatesRepository _templatesRepository;
        private readonly IMailSerivice _mailSerivice;

        public TeamsService(
            IEventsUsersRepository eventsUsersRepository,
            IEventsRepository eventsRepository,
            ITemplatesRepository templatesRepository,
            IMailSerivice mailSerivice
        ) 
        {
            _pairsRepository = eventsUsersRepository;
            _eventsRepository = eventsRepository;
            _templatesRepository = templatesRepository;
            _mailSerivice = mailSerivice;
        }

        public async Task<int> DistributeRandom(int eventId)
        {
            var ev = await _eventsRepository.GetEventAsync(eventId);

            if (ev == null)
                return 404;

            var pairs = await _pairsRepository.GetPairs(eventId);

            List<long?> ids = pairs
                            .Where(p => p.UserId != null)
                            .Select(p => p.UserId)
                            .ToList();

            using StringContent json =  new (
                JsonSerializer.Serialize(new 
                {
                    users = ids,
                    event_id = eventId
                }),
                Encoding.UTF8,
                "application/json"
            );
            var url = Environment.GetEnvironmentVariable("TEAMS_SERVICE_URL");
            var client = new HttpClient();
            var response = await client.PostAsync($"{url}/api/v1/withoutTeam", json);

            if (response.StatusCode.ToString() == "OK") 
            {
                var body = await response.Content.ReadFromJsonAsync<WithoutTeamDto>();
                var users = body.users;
                if (users.Count != 0) 
                {
                    var temp = await _templatesRepository.GetTemplate(eventId);
                    var teamNumbs = Math.Ceiling((double)users.Count / (double)temp.MaxLen);
                    var teams = new List<List<int>>();
                    bool flag = true;
                    for (int i = 0; i <= teamNumbs; i++) 
                    {
                        var toAdd = new List<int>();
                        for (int j = i * temp.MaxLen; j < temp.MaxLen * (i + 1); j++) 
                        {
                            toAdd.Add(users[j]);
                            if (j + 1 == users.Count) 
                            {
                                flag = false;
                                break;
                            }
                        }

                        teams.Add(toAdd);

                        if (!flag)
                            break;
                    }
                    string userUrl = Environment.GetEnvironmentVariable("USERS_SERVICE_URL");
                    for (int i = 0; i < teamNumbs; i++) 
                    {
                        int authorId = teams[i][0];
                        List<int> members = new List<int>();
                        string name = Guid.NewGuid().ToString();
                        name = $"Команда {name}";
                        for (int j = 0; j < teams[i].Count; j++) 
                        {
                            if (j == 0) 
                            {
                                authorId = teams[i][0];
                            }
                            else 
                            {
                                members.Add(teams[i][j]);
                            }
                            var resp = await client.GetFromJsonAsync<UserDto>($"{userUrl}/api/v1/users/get_user/{teams[i][j]}");
                            await _mailSerivice.SendAddedToTeam(resp.email, name, ev.Name);
                        }
                        using StringContent toCreate =  new (
                            JsonSerializer.Serialize(new 
                            {
                                author_id = authorId,
                                event_id = ev.Id,
                                name,
                                members
                            }),
                            Encoding.UTF8,
                            "application/json"
                        );
                        await client.PostAsync($"{url}/api/v1/autogenerateTeam", toCreate);
                    }
                }
            }

            return 0;
        }
    }
}