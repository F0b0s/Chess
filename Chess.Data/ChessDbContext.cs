using System;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Data.Migrations;

namespace Chess.Data
{
    public class ChessDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Login> Logins { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ChessDbContext, Configuration>());
            base.OnModelCreating(modelBuilder);
        }
    }
}
