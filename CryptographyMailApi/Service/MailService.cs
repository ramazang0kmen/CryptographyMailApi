using CryptographyMailApi.Data;
using CryptographyMailApi.Models;
using System.Net.Mail;

namespace CryptographyMailApi.Service
{
    public class MailService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;
        private readonly TokenService _tokenService;

        public MailService(ApplicationDbContext context, AuthService authService, TokenService tokenService)
        {
            _context = context;
            _authService = authService;
            _tokenService = tokenService;
        }

        public void SendMail(List<string> receiverMail, string subject, string message)
        {
            var sender = _tokenService.GetUserFromToken();
            if (sender is null)
                throw new Exception("Geçersiz kullanıcı.");

            foreach (var receiverMails in receiverMail)
            {
                var receiver = _authService.GetUserByMail(receiverMails);
                if (receiver is null)
                    throw new Exception("Alıcı bulunamadı.");

                var encryptedMessage = Helpers.EncryptionHelper.Encrypt(receiver.PublicKey, message);

                var mailMessage = new Mail()
                {
                    SenderId = sender.Id,
                    ReceiverId = receiver.Id,
                    Subject = subject,
                    EncryptedBody = encryptedMessage
                };

                _context.MailMessages.Add(mailMessage);
            }
            _context.SaveChanges();
        }

        public List<Mail> GetReceivedMails(int user)
        {
            return _context.MailMessages.Where(m => m.ReceiverId == user || m.SenderId == user).ToList();
        }

        public string DecryptMail(int user, int mailId, string privateKey)
        {
            var mail = _context.MailMessages.FirstOrDefault(m => m.Id == mailId && m.ReceiverId == user);
            if (mail is not null)
                return Helpers.EncryptionHelper.Decrypt(privateKey, mail.EncryptedBody);
            return null;
        }
    }
}
