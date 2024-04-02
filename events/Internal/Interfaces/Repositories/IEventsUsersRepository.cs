using Events.Internal.Storage.Entities;

namespace Events.Internal.Interafces 
{
    public interface IEventsUsersRepository 
    {
        public Task<EventsUsers> GetPair(long userId, int EventId);
        public Task<EventsUsers> GetPairByTg(string tgUsername, int eventId);
        public Task AddPair(EventsUsers pair);
        public Task UpdatePair(EventsUsers pair);
        public Task<List<EventsUsers>> GetPairs(int eventId);
    }
}