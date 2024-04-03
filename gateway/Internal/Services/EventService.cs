using System.Text;
using Gateway.Internal.Dto;
using Gateway.Internal.Interfaces;

namespace Gateway.Internal.Services 
{
    public class EventsService : IEventsService
    {
        private readonly IFilesService _filesService;

        public EventsService(IFilesService filesService) 
        {
            _filesService = filesService;
        }
        private readonly string url = Environment.GetEnvironmentVariable("EVENTS_SERVICE_URL");

        public async Task UploadUsers(int id, UploadUsers dto, string token)
        {
            var form = new MultipartFormDataContent();
            long length = dto.file.Length;
            using var fileStream = dto.file.OpenReadStream();
            //var name = await _filesService.UploadUserFile(dto.file);
            byte[] bytes = new byte[length];
            fileStream.Read(bytes, 0, (int)dto.file.Length);
            form.Add(new StreamContent(new MemoryStream(bytes)), "file", dto.file.FileName);
            form.Add(new StringContent("smlx,lsxsxl,ss", Encoding.UTF8), "membersPath");
            form.Add(new StringContent("1", Encoding.UTF8), "organizerId");
            var client = new HttpClient();
            var response = await client.PostAsync($"{url}/api/v1/events/{id}/upload", form);
        }

        public async Task<StatisticDto> GetStat(int id)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"{url}/api/v1/events/statistic/{id}");
            var res = await response.Content.ReadFromJsonAsync<StatisticDto>();
            return res;
        }
    }
}