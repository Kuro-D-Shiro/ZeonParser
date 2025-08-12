using Microsoft.EntityFrameworkCore;
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
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("category_pkey");
                entity.HasAlternateKey(e => e.Name)
                    .HasName("category_name_key");
                entity.HasAlternateKey(e => e.Link)
                    .HasName("category_link_key");

                entity.HasOne(e => e.ParentCategory)
                    .WithMany(p => p.ChildCategories)
                    .HasForeignKey(e => e.ParentCategoryId)
                    .HasConstraintName("category_parent_category_id_fkey");

                entity.HasMany(e => e.Products)
                    .WithOne(p => p.Category)
                    .HasForeignKey(p => p.CategoryId)
                    .HasConstraintName("product_category_id_fkey");

                entity.HasIndex(e => e.Name)
                    .IsUnique()
                    .HasName("idx_category_name_unique");
                entity.HasIndex(e => e.Link).IsUnique();
                entity.HasIndex(e => e.ParentCategoryId);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId);
                entity.HasAlternateKey(e => e.Name);
                entity.HasAlternateKey(e => e.Link);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(e => e.CategoryId);

                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.Link).IsUnique();
                entity.HasIndex(e => e.CategoryId);
                entity.HasIndex(e => e.CurrentPrice);
            });
        }
    }
}
