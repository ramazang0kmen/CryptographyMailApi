using CryptographyMailApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptographyMailApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Mail> MailMessages { get; set; }
    }
}
