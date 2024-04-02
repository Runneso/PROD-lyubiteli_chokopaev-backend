using System.Text;
using System.Text.Json;
using Gateway.Internal.Dto;
using Gateway.Internal.Interfaces;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Gateway.Internal.Services 
{
    public class FilessService : IFilesService
    {
        private readonly string url = Environment.GetEnvironmentVariable("FILES_SERVICE_URL");

        public async Task<FilesResult> UploadUserFile(IFormFile file)
        {
            var client = new HttpClient();

            var form = new MultipartFormDataContent();

            long length = file.Length;
            using var fileStream = file.OpenReadStream();
            byte[] bytes = new byte[length];
            fileStream.Read(bytes, 0, (int)file.Length);

            form.Add(new StreamContent(new MemoryStream(bytes)), "upload_file", file.FileName);

            var response = await client.PostAsync($"{url}/api/v1/files/upload_file", form);

            var result = new FilesResult();

            if (response.StatusCode.ToString() == "Created") 
            {
                result = await response.Content.ReadFromJsonAsync<FilesResult>();
            }

            return result;
        }
    }
}