using System.ComponentModel.DataAnnotations;

namespace CryptographyMailApi.Models
{
    public class Mail
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Subject { get; set; }
        public string EncryptedBody { get; set; }
        public DateTime SendAt { get; set; } = DateTime.UtcNow;
    }
}
