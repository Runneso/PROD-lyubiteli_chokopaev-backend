using Gateway.Internal.Dto;

namespace Gateway.Internal.Interfaces 
{
    public interface ITeamsService 
    {
        public Task<List<TeamDto>> GetTeamByEvent(int eventId, string token);
        public Task<TeamDto> GetTeamById(int id, string token);
        public Task CreateTeam(CreateTeamDto dto, string token);
        public Task DeleteTeam(int teamId, string token);
        public Task UpdateTeam(UpdateTeamDto dto, string token);
        Task CreateTag(CreateTagDto dto, string token);
        Task DeleteTag(int team_id, string tag, string token);
        Task CreateInvite(CreateInviteDto dto, string token);
        Task<List<InviteDto>> GetInvites(GetInviteDto dto, string token);
        Task AnswerInvite(AnswerInviteDto dto, string token);
        Task<List<TeamDto>> GetPossible(int eventId, int offset,string token);
        Task<TeamDto> My(int event_id, string token);
        Task DeleteUser(int t, int u, string token);
    }
}