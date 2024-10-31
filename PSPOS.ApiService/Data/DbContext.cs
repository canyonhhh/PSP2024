using Microsoft.EntityFrameworkCore;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<Address> Addresses { get; set; }
    }
}