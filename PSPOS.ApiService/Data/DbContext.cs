using Microsoft.EntityFrameworkCore;
using PSPOS.ServiceDefaults.Models;
namespace PSPOS.ApiService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Business> Businesses { get; set; }
}