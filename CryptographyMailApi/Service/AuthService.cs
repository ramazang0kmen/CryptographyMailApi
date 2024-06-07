using CryptographyMailApi.Data;
using CryptographyMailApi.Helpers;
using CryptographyMailApi.Models;
using System.Security.Cryptography;
using System.Text;

namespace CryptographyMailApi.Service
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        //private readonly FirebaseAuthHelper _firebaseAuthHelper;

        public AuthService(ApplicationDbContext context/*, FirebaseAuthHelper firebaseAuthHelper*/)
        {
            _context = context;
            //_firebaseAuthHelper = firebaseAuthHelper;
        }

        public (User user, string privateKey) Register(string email, string password)
        {
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            var (publicKey, privateKey) = Helpers.EncryptionHelper.GenerateKeyPair();

            var user = new User
            {
                Email = email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                PublicKey = publicKey
            };

            _context.Users.Add(user);
            _context.SaveChanges();
            //_firebaseAuthHelper.RegisterUser(user);

            return (user, privateKey);
        }

        public User Authenticate(string email, string password)
        {
            //var firebaseUser = _firebaseAuthHelper.LoginUser(email);
            var user = _context.Users.FirstOrDefault(u => u.Email.ToLower().Equals(email.ToLower()));
            if (user == null/* && firebaseUser == null*/)
                throw new System.Exception("Kullanıcı bulunamadı.");
                
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)/* && !VerifyPasswordHash(password, firebaseUser.PasswordHash, firebaseUser.PasswordSalt)*/)
                throw new System.Exception("Şifre hatalı.");

            return (user);
        }

        public User GetUserByMail(string userMail)
        {
            var receiver = _context.Users.FirstOrDefault(u => u.Email.Equals(userMail));
            //var firebaseReceiver = _firebaseAuthHelper.GetUserByMail(userMail);
            return receiver;
        }

        public User GetUserById(int userId)
        {
            var user = _context.Users.Find(userId);
            //var firebaseUser = _firebaseAuthHelper.GetUserById(userId);
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            };
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
