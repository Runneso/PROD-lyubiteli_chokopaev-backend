using Notifications.Internal.Dto;
using Notifications.Internal.Interfaces;
using System.Net.Mail;

namespace Notifications.Internal.Services 
{
    public class MailService : IMailService
    {
        private readonly SmtpClient _client;

        public MailService() 
        {
            _client = new SmtpClient()
            {
                
            };
        }

        public async void SendInvitationToApp(ToAppDto dto)
        {

        }
    }
}