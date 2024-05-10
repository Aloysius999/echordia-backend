using Ech.Executive.Authentication.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;

namespace Ech.Executive.Database
{
    public class MySQLDbContext : DbContext
    {
        public MySQLDbContext(DbContextOptions<MySQLDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        //public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("user");

            modelBuilder.Entity<User>()
                .Property(e => e.role)
                .HasConversion<string>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
