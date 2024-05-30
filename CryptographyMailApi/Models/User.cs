using System.ComponentModel.DataAnnotations;

namespace CryptographyMailApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }

        [Required]
        public string PublicKey { get; set; }
    }
}
