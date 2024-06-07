using CryptographyMailApi.Models;
using CryptographyMailApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CryptographyMailApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly MailService _mailService;
        private readonly TokenService _tokenService;

        public MailController(MailService mailService, TokenService tokenService)
        {
            _mailService = mailService;
            _tokenService = tokenService;
        }

        [HttpPost("send")]
        [Authorize]
        public IActionResult SendMail([FromBody] MailRequest request)
        {
            _mailService.SendMail(request.ReceiverMail, request.Subject, request.Message, request.PrivateKey);
            return Ok();
        }

        [HttpGet("received")]
        [Authorize]
        public IActionResult GetReceivedMails()
        {
            var user = _tokenService.GetUserFromToken();
            var mails = _mailService.GetReceivedMails(user.Id);
            return Ok(mails);
        }

        [HttpPost("decrypt")]
        [Authorize]
        public IActionResult DecryptMail([FromBody] DecryptRequest request)
        {
            var user = _tokenService.GetUserFromToken();
            var decryptedMessage = _mailService.DecryptMail(user.Id, request.MailId, request.PrivateKey);
            if (decryptedMessage is null)
                return BadRequest("Geçersiz şifre çözme anahtarı veya posta kimliği");

            return Ok(decryptedMessage);
        }

        [HttpGet("cryptomail")]
        [Authorize]
        public IActionResult GetCryptoMail(int mailId)
        {
            var user = _tokenService.GetUserFromToken();
            var mail = _mailService.GetCryptoMail(user.Id, mailId);

            if (mail is null)
                return BadRequest("Geçersiz posta kimliği");

            return Ok(mail);
        }
    }
    public class MailRequest
    {
        public List<String> ReceiverMail { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string PrivateKey { get; set; }
    }

    public class DecryptRequest
    {
        public int MailId { get; set; }
        public string PrivateKey { get; set; }
    }
}
