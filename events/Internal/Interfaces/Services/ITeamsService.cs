namespace Events.Internal.Interafces 
{
    public interface ITeamsService 
    {
        Task<int> DistributeRandom(int eventId);
    }
}