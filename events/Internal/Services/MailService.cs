using Events.Internal.Interafces;
using MailClient;
using Grpc.Net.Client;

namespace Events.Internal.Services 
{
    public class MailService : IMailSerivice
    {
        private readonly string url = Environment.GetEnvironmentVariable("MAIL_SERVICE_URL");

        public async Task SendInviteToApp(string email, string eventName)
        {
            using var chanel = GrpcChannel.ForAddress($"http://{url}");

            var client = new MailClient.MailService.MailServiceClient(chanel);

            await client.SendInvintationToAppAsync(new GetDataForm { Email = email, EventName = eventName });
        }

        public async Task SendInviteToEvent(string email, string eventName) 
        {
            using var chanel = GrpcChannel.ForAddress($"http://{url}");

            var client = new MailClient.MailService.MailServiceClient(chanel);

            await client.SendInvintationToEventAsync(new GetDataForm { Email = email, EventName = eventName });
        }
    }
}