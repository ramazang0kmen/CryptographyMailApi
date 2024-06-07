using System.Security.Cryptography;
using System.Text;

namespace CryptographyMailApi.Helpers
{
    public class EncryptionHelper
    {
        public static (string publicKey, string privateKey) GenerateKeyPair()
        {
            using (var rsa = RSA.Create(2048))
            {
                return (Convert.ToBase64String(rsa.ExportRSAPublicKey()), Convert.ToBase64String(rsa.ExportRSAPrivateKey()));
            }
        }

        public static string Encrypt(string publicKey, string message)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
                var encryptedMessage = rsa.Encrypt(Encoding.UTF8.GetBytes(message), RSAEncryptionPadding.OaepSHA256);
                return Convert.ToBase64String(encryptedMessage);
            }
        }

        public static string Decrypt(string privateKey, string encryptedMessage)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
                var decryptedMessage = rsa.Decrypt(Convert.FromBase64String(encryptedMessage), RSAEncryptionPadding.OaepSHA256);
                return Encoding.UTF8.GetString(decryptedMessage);
            }
        }

        public static string EncryptSing(string privateKey)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
                var encryptedMessage = rsa.SignData(Encoding.UTF8.GetBytes("Onaylandı"), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return Convert.ToBase64String(encryptedMessage);
            }
        }

        public static bool DecryptSing(string publicKey, string encryptedMessage)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
                var decryptedMessage = rsa.VerifyData(Encoding.UTF8.GetBytes("Onaylandı"), Convert.FromBase64String(encryptedMessage), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return decryptedMessage;
            }
        }
    }
}
