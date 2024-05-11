using Microsoft.EntityFrameworkCore;

namespace Ech.ItemSaleMonitor.Database
{
    public class MySQLDbContext : DbContext
    {
        public MySQLDbContext(DbContextOptions<MySQLDbContext> options) : base(options)
        {
        }

        //public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>().ToTable("user");

            //modelBuilder.Entity<User>()
            //    .Property(e => e.role)
            //    .HasConversion<string>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
