using Microsoft.EntityFrameworkCore;
using TelegramBotCore.DB.Model;

namespace TelegramBotCore.DB
{
    public class TelegramContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<DbMessage> DbMessages { get; set; }
        public DbSet<DbButton> DbButtons { get; set; }

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite ("Data Source=database.db");
        }
    }
}