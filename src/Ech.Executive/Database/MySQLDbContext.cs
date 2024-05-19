using Ech.Schema.Executive;
using Microsoft.EntityFrameworkCore;

namespace Ech.Executive.Database
{
    public class MySQLDbContext : DbContext
    {
        public MySQLDbContext(DbContextOptions<MySQLDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<SaleControl> SaleControls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // user
            modelBuilder.Entity<User>().ToTable("user");

            modelBuilder.Entity<User>()
                .Property(e => e.role)
                .HasConversion<string>();

            // saleControl
            //modelBuilder.Entity<SaleControl>().ToTable("saleControl");

            //modelBuilder.Entity<SaleControl>()
            //    .Property(e => e.saleType)
            //    .HasConversion<string>();
            //    .Property(e => e.runningStatus)
            //    .HasConversion<string>()
            //    .Property(e => e.sellingStatus)
            //    .HasConversion<string>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
