using CardGame.Models;
using Microsoft.EntityFrameworkCore;

namespace CardGame
{
    public class CardGameContext : DbContext
    {
        public DbSet<Card>? Cards { get; set; }
        public DbSet<Player>? Players { get; set; }
        public DbSet<Hand>? Hands { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-5CIA376\MSSQLSERVER01;Database=CardGameDB;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;");
        }
    }
}
