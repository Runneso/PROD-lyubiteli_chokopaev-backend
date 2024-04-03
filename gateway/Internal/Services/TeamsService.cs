using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Gateway.Internal.Dto;
using Gateway.Internal.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Internal.Services 
{
    public class TeamsService : ITeamsService 
    {
        private readonly string url = Environment.GetEnvironmentVariable("TEAMS_SERVICE_URL");
        private readonly IUsersService _usersService;

        public TeamsService(IUsersService usersService) 
        {
            _usersService = usersService;
        }
        

        public async Task<List<TeamDto>> GetTeamByEvent(int eventId, string token)
        {
            try 
            {
                await _usersService.GetProfile(token);
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }

            var client = new HttpClient();
            var response = await client.GetAsync($"{url}/api/v1/teams/{eventId}");

            if (response.StatusCode.ToString() == "UnprocessableEntity") 
            {
                throw new Exception("422");
            }
            else if (response.StatusCode.ToString() != "OK")
                throw new Exception("500");

            var teams = await response.Content.ReadFromJsonAsync<List<TeamResponse>>();
            List<TeamDto> result = new List<TeamDto>();
            foreach (var team in teams) 
            {
                var members = new List<ProfileDto>();
                foreach (var user in team.members) 
                {
                    ProfileDto member = new ProfileDto();
                    try 
                    {
                        member = await _usersService.GetProfileById(user, token);
                    }
                    catch (Exception ex) 
                    {
                        if (ex.Message == "404") 
                        {
                            await client.DeleteAsync($"{url}/api/v1/deleteMember?team_id={team.id}&user_id={user}");
                            continue;
                        }
                    }
                    members.Add(member);
                }
                TeamDto teamRes = new TeamDto
                {
                    id = team.id,
                    author_id = team.author_id,
                    event_id = team.event_id,
                    name = team.name,
                    size = team.size,
                    description = team.description,
                    need = team.need,
                    tags = team.tags,
                    members = members
                };
                result.Add(teamRes);
            }
            return result;
        }

        public async Task<TeamDto> GetTeamById(int id, string token)
        {
            try 
            {
                await _usersService.GetProfile(token);
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }

            var client = new HttpClient();
            var response = await client.GetAsync($"{url}/api/v1/team/{id}");
            if (response.StatusCode.ToString() == "NotFound") 
            {
                throw new Exception("404");
            }
            else if (response.StatusCode.ToString() == "UnprocessableEntity") 
            {
                throw new Exception("422");
            }
            else if (response.StatusCode.ToString() != "OK") 
            {
                throw new Exception("500");
            }
            var team = await response.Content.ReadFromJsonAsync<TeamResponse>();

            var members = new List<ProfileDto>();
            foreach (var user in team.members) 
            {
                ProfileDto member = new ProfileDto();
                try 
                {
                    member = await _usersService.GetProfileById(user, token);
                }
                catch (Exception ex) 
                {
                    if (ex.Message == "404") 
                    {
                        await client.DeleteAsync($"{url}/api/v1/deleteMember?team_id={team.id}&user_id={user}");
                        continue;
                    }
                }
                members.Add(member);
            }
            TeamDto result = new TeamDto
            {
                id = team.id,
                author_id = team.author_id,
                event_id = team.event_id,
                name = team.name,
                size = team.size,
                description = team.description,
                need = team.need,
                tags = team.tags,
                members = members
            };

            return result;
        }

        public async Task CreateTeam(CreateTeamDto dto, string token) 
        {
            ProfileDto me = new ProfileDto();
            try 
            {
                me = await _usersService.GetProfile(token);
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }

            using StringContent json =  new (
                JsonSerializer.Serialize(new 
                {
                    author_id = me.id,
                    event_id = dto.event_id,
                    name = dto.name,
                    size = dto.size,
                    description = dto.description,
                    need = dto.need
                }),
                Encoding.UTF8,
                "application/json"
            );

            var client = new HttpClient();
            var response = await client.PostAsync($"{url}/api/v1/create", json);
            if (response.StatusCode.ToString() == "UnprocessableEntity") 
            {
                throw new Exception("422");
            }
            else if (response.StatusCode.ToString() == "UnsupportedMediaType") 
            {
                throw new Exception("415");
            }
            else if (response.StatusCode.ToString() == "Conflict") 
            {
                throw new Exception("409");
            }
            else if (response.StatusCode.ToString() != "Created") 
            {
                throw new Exception("500");
            }
        }

        public async Task DeleteTeam(int teamId, string token)
        {
            try 
            {
                await _usersService.GetProfile(token);
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }

            var client = new HttpClient();
            var response = await client.DeleteAsync($"{url}/api/v1/remove?team_id={teamId}");  
            if (response.StatusCode.ToString() == "NotFound") 
            {
                throw new Exception("404");
            }     
            else if (response.StatusCode.ToString() != "NoContent") 
            {
                throw new Exception("500");
            }     
        }

        public async Task UpdateTeam(UpdateTeamDto dto, string token)
        {
            try 
            {
                await _usersService.GetProfile(token);
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            } 
            using StringContent json =  new (
                JsonSerializer.Serialize(new 
                {
                    team_id = dto.team_id,
                    name = dto.name,
                    size = dto.size,
                    description = dto.description,
                    need = dto.need
                }),
                Encoding.UTF8,
                "application/json"
            );
            var client = new HttpClient();
            var response = await client.PatchAsync($"{url}/api/v1/update", json); 
            if (response.StatusCode.ToString() == "NotFound") 
            {
                throw new Exception("404");
            }     
            else if (response.StatusCode.ToString() != "OK") 
            {
                throw new Exception("500");
            }     
        }

        public async Task CreateTag(CreateTagDto dto, string token)
        {
            try 
            {
                await _usersService.GetProfile(token);
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            } 
            using StringContent json =  new (
                JsonSerializer.Serialize(new 
                {
                    team_id = dto.team_id,
                    tag = dto.tag
                }),
                Encoding.UTF8,
                "application/json"
            );
            var client = new HttpClient();
            var response = await client.PostAsync($"{url}/api/v1/createTag", json); 
            if (response.StatusCode.ToString() == "Conflict") 
            {
                throw new Exception("409");
            }     
            else if (response.StatusCode.ToString() != "Created") 
            {
                throw new Exception("500");
            }
        }

        public async Task DeleteTag(int team_id, string tag, string token)
        {
            try 
            {
                await _usersService.GetProfile(token);
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
            var client = new HttpClient();
            var response = await client.DeleteAsync($"{url}/api/v1/deleteTag?team_id={team_id}&tag={tag}"); 
            if (response.StatusCode.ToString() == "Conflict") 
            {
                throw new Exception("409");
            }     
            else if (response.StatusCode.ToString() == "UnprocessableEntity") 
            {
                throw new Exception("422");
            }
            else if (response.StatusCode.ToString() != "NoContent") 
            {
                throw new Exception("500");
            }
        }

        public async Task CreateInvite(CreateInviteDto dto, string token)
        {
            try 
            {
                await _usersService.GetProfile(token);
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
            using StringContent json =  new (
                JsonSerializer.Serialize(dto),
                Encoding.UTF8,
                "application/json"
            );
            var client = new HttpClient();
            var response = await client.PostAsync($"{url}/api/v1/createInvite", json); 
            if (response.StatusCode.ToString() == "Conflict") 
            {
                throw new Exception("409");
            }     
            else if (response.StatusCode.ToString() == "Forbidden") 
            {
                throw new Exception("403");
            }
            else if (response.StatusCode.ToString() == "UnprocessableEntity") 
            {
                throw new Exception("422");
            }
            else if (response.StatusCode.ToString() != "Created") 
            {
                throw new Exception("500");
            }
        }

        public async Task<List<InviteDto>> GetInvites(GetInviteDto dto, string token)
        {
            ProfileDto me = new ProfileDto();   
            try 
            {
                me = await _usersService.GetProfile(token);
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
            var client = new HttpClient();
            var response = await client.GetAsync($"{url}/api/v1/invites?user_id={me.id}&event_id={dto.event_id}"); 
            if (response.StatusCode.ToString() != "OK") 
            {
                throw new Exception("500");
            }
            var invites = await response.Content.ReadFromJsonAsync<List<InviteDto>>();

            return invites; 
        }

        public async Task AnswerInvite(AnswerInviteDto dto, string token)
        {
            try 
            {
                await _usersService.GetProfile(token);
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
            using StringContent json =  new (
                JsonSerializer.Serialize(dto),
                Encoding.UTF8,
                "application/json"
            );
            var client = new HttpClient();
            var response = await client.PostAsync($"{url}/api/v1/answerInvite", json);
            if (response.StatusCode.ToString() != "OK") 
            {
                throw new Exception("500");
            }
        }

        public async Task<List<TeamDto>> GetPossible(int eventId, int offset, string token)
        {
            ProfileDto me = new ProfileDto(); 
            try 
            {
                me = await _usersService.GetProfile(token);
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
            var client = new HttpClient();
            var response = await client.GetAsync($"{url}/api/v1/possibleTeams?event_id={eventId}&user_id={me.id}&offset={offset}");
            if (response.StatusCode.ToString() != "OK") 
            {
                throw new Exception("500");
            }
            var teams = await response.Content.ReadFromJsonAsync<List<TeamResponse>>();
            var result = new List<TeamDto>();
            foreach(var team in teams) 
            {
                var members = new List<ProfileDto>();
                foreach (var user in team.members) 
                {
                    ProfileDto member = new ProfileDto();
                    try 
                    {
                        member = await _usersService.GetProfileById(user, token);
                    }
                    catch (Exception ex) 
                    {
                        if (ex.Message == "404") 
                        {
                            await client.DeleteAsync($"{url}/api/v1/deleteMember?team_id={team.id}&user_id={user}");
                            continue;
                        }
                    }
                    members.Add(member);
                }
                TeamDto r = new TeamDto
                {
                    id = team.id,
                    author_id = team.author_id,
                    event_id = team.event_id,
                    name = team.name,
                    size = team.size,
                    description = team.description,
                    need = team.need,
                    tags = team.tags,
                    members = members
                };
                result.Add(r);
            }
            return result;
        }

        public async Task<TeamDto> My(int event_id, string token)
        {
            ProfileDto me = new ProfileDto(); 
            try 
            {
                me = await _usersService.GetProfile(token);
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
            using StringContent json =  new (
                JsonSerializer.Serialize(new 
                {
                    user_id = me.id,
                    event_id = event_id
                }),
                Encoding.UTF8,
                "application/json"
            );
            var client = new HttpClient();
            var response = await client.PostAsync($"{url}/api/v1/myTeam", json);
            if (response.StatusCode.ToString() != "OK") 
            {
                throw new Exception("500");
            }
            var team = await response.Content.ReadFromJsonAsync<TeamResponse>();

            var members = new List<ProfileDto>();
            foreach (var user in team.members) 
            {
                ProfileDto member = new ProfileDto();
                try 
                {
                    member = await _usersService.GetProfileById(user, token);
                }
                catch (Exception ex) 
                {
                    if (ex.Message == "404") 
                    {
                        await client.DeleteAsync($"{url}/api/v1/deleteMember?team_id={team.id}&user_id={user}");
                        continue;
                    }
                }
                members.Add(member);
            }
            TeamDto result = new TeamDto
            {
                id = team.id,
                author_id = team.author_id,
                event_id = team.event_id,
                name = team.name,
                size = team.size,
                description = team.description,
                need = team.need,
                tags = team.tags,
                members = members
            };

            return result;
        }

        public async Task DeleteUser(int t, int u, string token)
        {
            try 
            {
                await _usersService.GetProfile(token);
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
            var client = new HttpClient();
            var response = await client.DeleteAsync($"{url}/api/v1/deleteMember?team_id={t}&user_id={u}");
            if (response.StatusCode.ToString() != "NoContent") 
            {
                throw new Exception("500");
            }
        }
    }
}