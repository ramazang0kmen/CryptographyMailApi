using CryptographyMailApi.Models;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace CryptographyMailApi.Helpers
{
    public class FirebaseAuthHelper
    {
        IFirebaseClient client;

        public FirebaseAuthHelper()
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "VzO1k3Jv02goTFmmUFkBsdBf4axQcNtDGjOq2kHA",
                BasePath = "https://cryptographymailapi-default-rtdb.europe-west1.firebasedatabase.app/"
            };

            client = new FireSharp.FirebaseClient(config);

            if (client == null)
            {
                throw new System.Exception("Firebase bağlantısı kurulamadı.");
            }
        }

        

        public void RegisterUser(User user)
        {
            //SetResponse response = null;
            //User result = null;

            //var firebaseUser = new User
            //{
            //    Id = user.Id,
            //    Email = user.Email,
            //    PasswordHash = user.PasswordHash,
            //    PasswordSalt = user.PasswordSalt,
            //    PublicKey = user.PublicKey
            //};

            //response = client.Set("users/" + user.Id, firebaseUser);
            //result = response.ResultAs<User>();

            //if (result == null)
            //{
            //    throw new System.Exception("Kullanıcı kaydedilemedi.");
            //}
            PushResponse response = client.Push("users/", user);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new System.Exception("Kullanıcı kaydedilemedi.");
            }
        }

        public User LoginUser(string email)
        {
            FirebaseResponse response = client.Get("users/" + email);
            User user = response.ResultAs<User>();

            if (user == null)
            {
                throw new System.Exception("Kullanıcı bulunamadı.");
            }

            return user;
        }

        public User GetUserByMail(string userMail)
        {
            FirebaseResponse response = client.Get("users/" + userMail);
            User user = response.ResultAs<User>();

            if (user == null)
            {
                throw new System.Exception("Kullanıcı bulunamadı.");
            }

            return user;
        }

        public User GetUserId(int id)
        {
            FirebaseResponse response = client.Get("users/" + id);
            User user = response.ResultAs<User>();

            if (user == null)
            {
                throw new System.Exception("Kullanıcı bulunamadı.");
            }

            return user;
        }
    }
}
