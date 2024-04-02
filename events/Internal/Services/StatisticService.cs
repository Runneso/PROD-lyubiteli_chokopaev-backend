using Aspose.Cells;
using Events.Internal.Dto;
using Events.Internal.Interafces;

namespace Events.Internal.Services 
{
    public class StatisticService : IStatisticService 
    {
        private readonly IEventsUsersRepository _pairsRepository;
        private readonly IEventsRepository _eventsRepository;

        public StatisticService(
            IEventsUsersRepository eventsUsersRepository,
            IEventsRepository eventsRepository
        ) 
        {
            _pairsRepository = eventsUsersRepository;
            _eventsRepository = eventsRepository;
        }

        public async Task<StatisticDto> GetStatistic(int eventId) 
        {
            var ev = await _eventsRepository.GetEventAsync(eventId);

            if (ev == null)
                throw new Exception("404");

            var result = new StatisticDto();

            int backCnt = 0;
            int frontCnt = 0;
            int mobileCnt = 0;
            int dataCnt = 0;
            int fullCnt = 0;

            int py = 0;
            int js = 0;
            int cpp = 0;
            int cs = 0;
            int go = 0;
            int ru = 0;

            var pairs  = await _pairsRepository.GetPairs(eventId);

            var client = new HttpClient();
            string url = Environment.GetEnvironmentVariable("USERS_SERVICE_URL");

            foreach (var p in pairs) 
            {
                if (p.IsJoin == false) 
                {
                    continue;
                }

                var response = await client.GetAsync($"{url}/api/v1/users/get_user/{p.UserId}");

                if (response.StatusCode.ToString() == "OK") 
                {
                    var user = await response.Content.ReadFromJsonAsync<UserDto>();

                    if (user.role == "backend") 
                    {
                        backCnt += 1;
                    } 
                    else if (user.role == "frontend") 
                    {
                        frontCnt += 1;
                    }
                    else if (user.role == "mobile") 
                    {
                        mobileCnt += 1;
                    }
                    else if (user.role == "fullstack") 
                    {
                        fullCnt += 1;
                    }
                    else if (user.role == "datascience") 
                    {
                        dataCnt += 1;
                    }

                    foreach (var lang in user.langs) 
                    {
                        if (lang == "python") 
                        {
                            py += 1;
                        }
                        else if (lang == "rust") 
                        {
                            ru += 1;
                        }
                        else if (lang == "js") 
                        {
                            js += 1;
                        }
                        else if (lang == "cs") 
                        {
                            cs += 1;
                        }
                        else if (lang == "cpp") 
                        {
                            cpp += 1;
                        }
                        else if (lang == "go") 
                        {
                            go += 1;
                        }
                    }
                }
            }

            result.BackendPercentages = (double)backCnt / (double)pairs.Count * (double)100;
            result.FrontendPercentages = (double)frontCnt / (double)pairs.Count * (double)100;
            result.MobilePercentages = (double)mobileCnt / (double)pairs.Count * (double)100;
            result.DataSciencePercentages = (double)dataCnt / (double)pairs.Count * (double)100;
            result.FullStackPercentages = (double)fullCnt / (double)pairs.Count * (double)100;

            result.PythonPercentages = (double)py / (double)pairs.Count * (double)100;
            result.CpptPercentages = (double)cpp / (double)pairs.Count * (double)100;
            result.CsharpPercentages = (double)cs / (double)pairs.Count * (double)100;
            result.GolangPercentages = (double)go / (double)pairs.Count * (double)100;
            result.RustPercentages = (double)ru / (double)pairs.Count * (double)100;
            result.JSAndTSPercentages = (double)js / (double)pairs.Count * (double)100;

            return result;
        }
    }
}