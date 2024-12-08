using Microsoft.EntityFrameworkCore;
using PSPOS.ApiService.Data.ValueConverters;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Business> Businesses { get; set; }

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