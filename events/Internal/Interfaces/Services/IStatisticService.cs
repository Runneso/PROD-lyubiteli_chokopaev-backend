using Events.Internal.Dto;

namespace Events.Internal.Interafces
{
    public interface IStatisticService 
    {
        Task<StatisticDto> GetStatistic(int eventId);
    }
}