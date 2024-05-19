using Ech.Schema.Executive;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
            //var converter = new ValueConverter<User.Role, string>(
            //    v => v.ToString(),
            //    v => (User.Role)Enum.Parse(typeof(User.Role), v));

            // user
            modelBuilder.Entity<User>().ToTable("user");

            modelBuilder.Entity<User>()
                .Property(e => e.role)
                .HasConversion(new ValueConverter<User.Role, string>(
                    v => v.ToString(),
                    v => (User.Role)Enum.Parse(typeof(User.Role), v)));

            // saleControl
            modelBuilder.Entity<SaleControl>().ToTable("saleControl");

            modelBuilder.Entity<SaleControl>()
                .Property(e => e.saleType)
                .HasConversion(new ValueConverter<SaleControl.SaleType, string>(
                    v => v.ToString(),
                    v => (SaleControl.SaleType)Enum.Parse(typeof(SaleControl.SaleType), v)));
            modelBuilder.Entity<SaleControl>()
                .Property(e => e.runningStatus)
                .HasConversion(new ValueConverter<SaleControl.RunningStatus, string>(
                    v => v.ToString(),
                    v => (SaleControl.RunningStatus)Enum.Parse(typeof(SaleControl.RunningStatus), v)));
            modelBuilder.Entity<SaleControl>()
                .Property(e => e.sellingStatus)
                .HasConversion(new ValueConverter<SaleControl.SellingStatus, string>(
                    v => v.ToString(),
                    v => (SaleControl.SellingStatus)Enum.Parse(typeof(SaleControl.SellingStatus), v)));


            base.OnModelCreating(modelBuilder);
        }
    }
}
