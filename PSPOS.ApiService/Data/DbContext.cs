using Microsoft.EntityFrameworkCore;
using PSPOS.ServiceDefaults.Models;
namespace PSPOS.ApiService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<GiftCard> GiftCards { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Business> Businesses { get; set; }

}
