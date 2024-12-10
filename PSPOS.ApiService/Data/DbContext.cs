using Microsoft.EntityFrameworkCore;
using PSPOS.ApiService.Data.ValueConverters;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Giftcard> GiftCards { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Business> Businesses { get; set; }

    public DbSet<Tax> Taxes { get; set; }

    public DbSet<Reservation> Reservations { get; set; }

    public DbSet<Service> Services { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductGroup> ProductGroups { get; set; }
    public DbSet<ServiceGroup> ServiceGroups { get; set; }
    public DbSet<ProductStock> ProductStocks { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // apply converters for DateTime and DateTime? properties
        var utcDateTimeConverter = new UtcDateTimeConverter();
        var utcNullableDateTimeConverter = new UtcNullableDateTimeConverter();

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(utcDateTimeConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(utcNullableDateTimeConverter);
                }
            }
        }

        base.OnModelCreating(modelBuilder);
    }
}