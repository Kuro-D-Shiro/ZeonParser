using Microsoft.EntityFrameworkCore;
using ZeonService.Data.EntityConfiguration;
using ZeonService.Models;

namespace ZeonService.Data
{
    public class ZeonDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;

        public ZeonDbContext(DbContextOptions<ZeonDbContext> options)
            : base(options)
        {
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
        }
    }
}
