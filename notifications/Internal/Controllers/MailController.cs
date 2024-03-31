using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Notifications.Internal.Dto;
using Notifications.Internal.Interfaces;

namespace Notifications.Internal.Controllers 
{
    [ApiController]
    [Route("mail")]
    public class MailController : Controller 
    {
        private readonly IMailService _mailService;

        public MailController(IMailService mailService) 
        {
            _mailService = mailService;
        }

        [HttpPost("toapp")]
        public async Task<IActionResult> SendInvitationToApp([FromBody] ToAppDto dto) 
        {
            try 
            {
                _mailService.SendInvitationToApp(dto);
            }
            catch (Exception ex) 
            {
                return new StatusCodeResult(500);
            }

            return new OkResult();
        }
    }
}