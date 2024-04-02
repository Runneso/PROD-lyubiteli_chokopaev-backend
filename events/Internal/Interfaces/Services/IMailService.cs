namespace Events.Internal.Interafces 
{
    public interface IMailSerivice 
    {
        Task SendInviteToApp(string email, string eventName);
        Task SendInviteToEvent(string email, string eventName);
        Task SendAddedToTeam(string email, string teamName, string eventName);
    }
}