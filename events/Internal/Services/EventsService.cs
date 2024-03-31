using Events.Internal.Interafces;
using Events.Internal.Storage.Entities;
using Aspose.Cells;

namespace Events.Internal.Services 
{
    public class EventsService : IEventsService 
    {

        private readonly IEventsRepository _eventsRepository;
       // private readonly Microsoft.Office.Interop.Excel.Application _excel;
        
        public EventsService(
            IEventsRepository eventsRepository
        ) 
        {
            _eventsRepository = eventsRepository;
            //_excel = new Microsoft.Office.Interop.Excel.Application();
        }

        public async Task<Event> GetEvent(int id) 
        {
            var result = await _eventsRepository.GetEvent(id);
            
            return result;
        }

        public async void UploadMembers(int id, IFormFile file)
        {
            string path = Path.Combine("Files", $"{Guid.NewGuid().ToString()}.xlsx");
            using FileStream stream = File.Create(path);
            await file.CopyToAsync(stream);
            Workbook workbook = new Workbook(path);
            Worksheet worksheet = workbook.Worksheets[0];
            List<string> emails = new List<string>();
            for (int i = 0; i < worksheet.Cells.Rows.Count; i++) 
            {
                string email = worksheet.Cells[i + 1, 5].Value.ToString();
                emails.Add(email);
            } 
        }
    }
}