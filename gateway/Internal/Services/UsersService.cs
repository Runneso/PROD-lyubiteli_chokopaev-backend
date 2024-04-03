using System.Diagnostics;
using System.Net.Mime;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Xml;
using Gateway.Internal.Dto;
using Gateway.Internal.Interfaces;

namespace Gateway.Internal.Services 
{
    public class UsersService : IUsersService
    {
        private readonly string url = Environment.GetEnvironmentVariable("USERS_SERVICE_URL");
        private readonly IFilesService _filesService;

        public UsersService(IFilesService filesService) 
        {
            _filesService = filesService;
        }

        public async Task<TokenResult> CreateUser(CreateUserDto dto)
        {
            var toSend = new UserDto 
            {
                name = dto.name,
                surname = dto.surname,
                email = dto.email,
                password = dto.password,
                tg_username = dto.tg_username,
                is_admin = dto.is_admin
            };
            if (dto.patronymic != null) 
                toSend.patronymic = dto.patronymic;

            if (dto.photoFile != null) 
            {
                var fileUrl = await _filesService.UploadUserFile(dto.photoFile);
                toSend.photo = fileUrl.url;
            }

            if (dto.role != null)
                toSend.role = dto.role;

            if (dto.langs != null)
                toSend.langs = dto.langs;

            if (dto.tags != null) 
                toSend.tags = dto.tags;
            
            var client = new HttpClient();

            using StringContent toCreate =  new (
                JsonSerializer.Serialize(toSend),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync($"{url}/api/v1/users/create_user", toCreate);
            var result = new TokenResult();
            if (response.StatusCode.ToString() == "Created") 
            {
                result = await response.Content.ReadFromJsonAsync<TokenResult>();

                return result;
            }
            else if (response.StatusCode.ToString() == "Conflict")
            {
                throw new Exception("409");
            }
            else if (response.StatusCode.ToString() == "NotFound")
            {
                throw new Exception("404");
            }
            else if (response.StatusCode.ToString() == "UnprocessableEntity") 
            {
                throw new Exception("422");
            }
            else
                throw new Exception("500");
        }

        public async  Task DeleteUser(string token)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", token);
            var response = await client.DeleteAsync($"{url}/api/v1/users/delete_me");

            if (response.StatusCode.ToString() == "Unauthorized") 
            {
                throw new Exception("401");
            }
            else if (response.StatusCode.ToString() == "NotFound")
            {
                throw new Exception("404");
            }
            else if (response.StatusCode.ToString() == "UnprocessableEntity") 
            {
                throw new Exception("422");
            }
            else if (response.StatusCode.ToString() == "Conflict") 
            {
                throw new Exception("409");
            }
        }

        public async Task<ProfileDto> GetProfile(string token)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", token);
            var response = await client.GetAsync($"{url}/api/v1/users/get_me");

            if (response.StatusCode.ToString() == "Unauthorized") 
            {
                throw new Exception("401");
            }
            else if (response.StatusCode.ToString() == "NotFound")
            {
                throw new Exception("404");
            }
            else if (response.StatusCode.ToString() == "UnprocessableEntity") 
            {
                throw new Exception("422");
            }
            else if (response.StatusCode.ToString() == "Conflict") 
            {
                throw new Exception("409");
            }

            var result = await response.Content.ReadFromJsonAsync<ProfileDto>();
            return result;
        }

        public async Task<ProfileDto> GetProfileById(int id, string token)
        {
            try 
            {
                var user = await GetProfile(token);
            }
            catch (Exception ex) 
            {
                if (ex.Message == "401")
                    throw new Exception("401");
                else if (ex.Message == "404")
                    throw new Exception("404");
            }

            var client = new HttpClient();
            var response = await client.GetAsync($"{url}/api/v1/users/get_user/{id}");
            Console.WriteLine(response.StatusCode.ToString());
            if (response.StatusCode.ToString() == "NotFound") 
                throw new Exception("404");
            else if (response.StatusCode.ToString() == "UnprocessableEntity")
                throw new Exception("422");
            var profile = await response.Content.ReadFromJsonAsync<ProfileDto>(); 

            return profile;
        }

        public async Task<TokenResult> Login(LoignDto dto)
        {
            var form = new MultipartFormDataContent();
            form.Add(new StringContent(dto.Email, Encoding.UTF8, MediaTypeNames.Application.FormUrlEncoded), "username");
            form.Add(new StringContent(dto.Password, Encoding.UTF8, MediaTypeNames.Application.FormUrlEncoded), "password");
            var client = new HttpClient();

            var response = await client.PostAsync($"{url}/api/v1/users/sign_in", form);
            if (response.StatusCode.ToString() == "OK") 
            {
                var result = await response.Content.ReadFromJsonAsync<TokenResult>();

                return result;
            }
            else if (response.StatusCode.ToString() == "Unauthorized") 
            {
                throw new Exception("401"); 
            }
            else if (response.StatusCode.ToString() == "NotFound")
            {
                throw new Exception("404");
            }
            else if (response.StatusCode.ToString() == "UnprocessableEntity") 
            {
                throw new Exception("422");
            }
            else
            {
                throw new Exception("500");
            }
        }

        public async Task<ProfileDto> UpdateUser(UpdateProfileDto dto, string token)
        {
            string photoUrl = null;

            if (dto.photo != null) 
            {
                var r = await _filesService.UploadUserFile(dto.photo);
                photoUrl = r.url;   
            } 
            
            var toUpdate = new UpdateDto 
            {
                name = dto.name,
                surname = dto.surname,
                patronymic = dto.patronymic,
                photo = photoUrl,
                tg_username = dto.tg_username,
                role = dto.role,
                langs = dto.langs,
                tags = dto.tags
            };

            using StringContent json =  new (
                JsonSerializer.Serialize(toUpdate),
                Encoding.UTF8,
                "application/json"
            );

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", token);
            var response = await client.PatchAsync($"{url}/api/v1/users/patch_me", json);
            if (response.StatusCode.ToString() == "OK") 
            {
                var result = await response.Content.ReadFromJsonAsync<ProfileDto>();

                return result;
            }
            else if (response.StatusCode.ToString() == "Unauthorized") 
            {
                throw new Exception("401"); 
            }
            else if (response.StatusCode.ToString() == "NotFound")
            {
                throw new Exception("404");
            }
            else if (response.StatusCode.ToString() == "UnprocessableEntity") 
            {
                throw new Exception("422");
            }
            else if (response.StatusCode.ToString() == "Conflict") 
            {
                throw new Exception("409");
            }
            else
            {
                throw new Exception("500");
            }
        } 
    }
}