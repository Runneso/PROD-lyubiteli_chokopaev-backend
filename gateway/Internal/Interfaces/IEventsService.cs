using Gateway.Internal.Dto;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace Gateway.Internal.Interfaces 
{
    public interface IEventsService 
    {
        public Task UploadUsers(int id, UploadUsers dto, string token);
        Task<StatisticDto> GetStat(int id);
    }
}