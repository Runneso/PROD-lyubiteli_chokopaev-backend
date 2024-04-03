using Gateway.Internal.Dto;
using Gateway.Internal.Interfaces;

namespace Gateway.Internal.Services 
{
    public class EventsService : IEventsService
    {
        private readonly string url = Environment.GetEnvironmentVariable("EVENTS_SERVICE_URL");
        public async Task<List<EventDto>> GetEvents()
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"{url}/api/v1/events");
            if (response.StatusCode.ToString() != "OK") 
            {
                throw new Exception("500");
            }
            var result = await response.Content.ReadFromJsonAsync<List<EventDto>>();
            
            return result;
        }
    }
}