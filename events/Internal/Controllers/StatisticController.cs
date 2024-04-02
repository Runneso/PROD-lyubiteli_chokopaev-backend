using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using Events.Internal.Interafces;
using Microsoft.AspNetCore.Mvc;

namespace Events.Internal.Controllers 
{
    [ApiController]
    [Route("api/v1/events/statistic")]
    public class StatisticController : Controller 
    {
        private readonly IStatisticService _statisticService;

        public StatisticController(IStatisticService statisticService) 
        {
            _statisticService = statisticService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStatistic(int id) 
        {
            try 
            {
                var result = await _statisticService.GetStatistic(id);

                return new JsonResult(result);
            }
            catch (Exception ex) 
            {
                if (ex.Message == "404")
                    return new NotFoundResult();
                else 
                    return new StatusCodeResult(500);
            }
        }
    }
}