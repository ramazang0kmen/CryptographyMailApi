using CryptographyMailApi.Models;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;

namespace CryptographyMailApi.Helpers
{
    public class FirebaseMailHelper
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "VzO1k3Jv02goTFmmUFkBsdBf4axQcNtDGjOq2kHA",
            BasePath = "https://console.firebase.google.com/u/1/project/cryptographymailapi/database/cryptographymailapi-default-rtdb/data/~2F"
        };

        IFirebaseClient client;

        public void SendMail(Mail mail)
        {
            SetResponse response = null;
            Mail result = null;

            client = new FireSharp.FirebaseClient(config);

            var firebaseMail = new Mail
            {
                Id = mail.Id,
                SenderId = mail.SenderId,
                ReceiverId = mail.ReceiverId,
                Subject = mail.Subject,
                EncryptedBody = mail.EncryptedBody,
                SendAt = mail.SendAt
            };

            response = client.Set("mails/" + mail.Id, firebaseMail);
            result = response.ResultAs<Mail>();

            if (result == null)
            {
                throw new System.Exception("Mail gönderilemedi.");
            }
        }

        public List<Mail> GetMails(int user)
        {
            FirebaseResponse response = client.Get("mails");
            List<Mail> listMail = new List<Mail>();

            client = new FireSharp.FirebaseClient(config);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = response.Body;
                var data = JsonConvert.DeserializeObject<Dictionary<string, Mail>>(result);

                foreach (var item in data)
                {
                    Mail mail = item.Value;
                    listMail.Add(mail);
                }

                // Alıcı ID'sine göre filtreleme
                var filteredMails = listMail.Where(m => m.ReceiverId == user).ToList();
                return filteredMails;
            }

            return new List<Mail>();
        }

        public Mail GetMail(int mailId)
        {
            FirebaseResponse response = client.Get("mails/" + mailId);
            Mail mail = response.ResultAs<Mail>();

            return mail;
        }
    }
}
