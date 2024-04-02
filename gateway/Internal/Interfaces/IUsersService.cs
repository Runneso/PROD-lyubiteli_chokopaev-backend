using Gateway.Internal.Dto;

namespace Gateway.Internal.Interfaces 
{
    public interface IUsersService 
    {
        Task<TokenResult> CreateUser(CreateUserDto dto);
        Task<TokenResult> Login(LoignDto dto);
        Task<ProfileDto> GetProfile(string token);
        Task<ProfileDto> GetProfileById(int id, string token);
        Task<ProfileDto> UpdateUser(UpdateProfileDto dto, string token);
        Task DeleteUser(string token);
    }
}