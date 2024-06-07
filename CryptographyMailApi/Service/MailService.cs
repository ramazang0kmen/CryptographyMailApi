using CryptographyMailApi.Data;
using CryptographyMailApi.Helpers;
using CryptographyMailApi.Models;
using FireSharp.Interfaces;
using Newtonsoft.Json;
using System.Net.Mail;

namespace CryptographyMailApi.Service
{
    public class MailService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;
        private readonly TokenService _tokenService;
        //private readonly FirebaseMailHelper _firebaseMailHelper;
        //private readonly FirebaseAuthHelper _firebaseAuthHelper;

        public MailService(ApplicationDbContext context, AuthService authService, TokenService tokenService/*, FirebaseMailHelper firebaseMailHelper, FirebaseAuthHelper firebaseAuthHelper*/)
        {
            _context = context;
            _authService = authService;
            _tokenService = tokenService;
            //_firebaseMailHelper = firebaseMailHelper;
            //_firebaseAuthHelper = firebaseAuthHelper;
        }

        public void SendMail(List<string> receiverMail, string subject, string message, string privateKey)
        {
            var sender = _tokenService.GetUserFromToken();
            if (sender is null)
                throw new Exception("Geçersiz kullanıcı.");

            foreach (var receiverMails in receiverMail)
            {
                var receiver = _authService.GetUserByMail(receiverMails);
                //var firebaseReceiver = _firebaseAuthHelper.GetUserByMail(receiverMails);

                if (receiver is null)
                    throw new Exception("Alıcı bulunamadı.");

                var encryptedMessage = Helpers.EncryptionHelper.Encrypt(receiver.PublicKey, message);
                var signature = Helpers.EncryptionHelper.EncryptSing(privateKey);

                var mailMessage = new Mail()
                {
                    SenderId = sender.Id,
                    ReceiverId = receiver.Id,
                    Subject = subject,
                    EncryptedBody = encryptedMessage,
                    Approv = signature
                    
                };
                _context.MailMessages.Add(mailMessage);
                //_firebaseMailHelper.SendMail(mailMessage);
            }
            _context.SaveChanges();
        }

        public List<Mail> GetReceivedMails(int user)
        {
            var mails = _context.MailMessages.Where(m => m.ReceiverId == user).ToList();
            //var firebasemails = _firebaseMailHelper.GetMails(user);
            return mails;
        }

        public string DecryptMail(int user, int mailId, string privateKey)
        {
            var mail = _context.MailMessages.FirstOrDefault(m => m.Id == mailId && m.ReceiverId == user);
            //var firebasemail = _firebaseMailHelper.GetMail(mailId);
            if (mail is not null)
            {
                var sender = _authService.GetUserById(mail.SenderId);
                var decryptedMessage = Helpers.EncryptionHelper.Decrypt(privateKey, mail.EncryptedBody);
                var decryptedSignature = Helpers.EncryptionHelper.DecryptSing(sender.PublicKey, mail.Approv);
                if (!decryptedSignature)
                    return "Şüpheli bir mail algılandı";
                var mailInfo = new
                {
                    MailId = mail.Id,
                    Sender = mail.SenderId,
                    Receiver = mail.ReceiverId,
                    Subject = mail.Subject,
                    Message = decryptedMessage
                };

                return JsonConvert.SerializeObject(mailInfo);
            }
            return null;
        }

        public string GetCryptoMail(int user, int mailId)
        {
            var mail = _context.MailMessages.FirstOrDefault(m => m.Id == mailId && m.ReceiverId == user);
            if (mail is not null)
            {
                var mailInfo = new
                {
                    MailId = mail.Id,
                    Sender = mail.SenderId,
                    Receiver = mail.ReceiverId,
                    Subject = mail.Subject,
                    Message = mail.EncryptedBody,
                    Approv = mail.Approv
                };

                return JsonConvert.SerializeObject(mailInfo);
            }
            return null;
        }
    }
}
