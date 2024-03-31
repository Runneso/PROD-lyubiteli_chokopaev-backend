using Notifications.Internal.Dto;

namespace Notifications.Internal.Interfaces 
{
    public interface IMailService 
    {
        void SendInvitationToApp(ToAppDto  dto);
    }
}