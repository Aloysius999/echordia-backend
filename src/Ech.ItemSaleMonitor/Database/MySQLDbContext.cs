using Ech.Schema.Executive;
using Ech.Schema.IntraService.ItemSaleMonitor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ech.ItemSaleMonitor.Database
{
    public class MySQLDbContext : DbContext
    {
        public MySQLDbContext(DbContextOptions<MySQLDbContext> options) : base(options)
        {
            try
            {
                // ensure the DB is created
                bool res = Database.EnsureCreated();
            }
            catch (Exception ex)
            {
            }
        }

        public DbSet<ItemSaleControl> ItemSaleControls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            try
            {
                modelBuilder.Entity<ItemSaleControl>().ToTable("itemSaleControl");

                modelBuilder.Entity<ItemSaleControl>()
                    .Property(e => e.saleType)
                    .HasConversion(new ValueConverter<SaleControl.SaleType, string>(
                        v => v.ToString(),
                        v => (SaleControl.SaleType)Enum.Parse(typeof(SaleControl.SaleType), v)));
                modelBuilder.Entity<ItemSaleControl>()
                    .Property(e => e.runningStatus)
                    .HasConversion(new ValueConverter<SaleControl.RunningStatus, string>(
                        v => v.ToString(),
                        v => (SaleControl.RunningStatus)Enum.Parse(typeof(SaleControl.RunningStatus), v)));
                modelBuilder.Entity<ItemSaleControl>()
                    .Property(e => e.sellingStatus)
                    .HasConversion(new ValueConverter<SaleControl.SellingStatus, string>(
                        v => v.ToString(),
                        v => (SaleControl.SellingStatus)Enum.Parse(typeof(SaleControl.SellingStatus), v)));

                base.OnModelCreating(modelBuilder);
            }
            catch (Exception ex)
            { 
            }
        }
    }
}
