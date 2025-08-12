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
                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id");
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("name");
                entity.Property(e => e.Link)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("link");
                entity.Property(e => e.ParentCategoryId)
                    .HasColumnName("parent_category_id");

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
                    .HasConstraintName("product_category_id_fkey")
                    .OnDelete(DeleteBehavior.Cascade);

                //помнится мне, что для пк бдшка сама создаёт индекс
                entity.HasIndex(e => e.Name)
                    .IsUnique()
                    .HasDatabaseName("idx_category_name_unique");
                entity.HasIndex(e => e.Link)
                    .IsUnique()
                    .HasDatabaseName("idx_category_link_unique");
                entity.HasIndex(e => e.ParentCategoryId)
                    .HasDatabaseName("idx_category_parent_category_id_fkey");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId)
                    .HasName("product_pkey");
                entity.HasAlternateKey(e => e.Link)
                    .HasName("product_link_key");
                entity.HasAlternateKey(e => e.Name)
                    .HasName("product_name_key");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id");
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("name");
                entity.Property(e => e.Link)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("link");
                entity.Property(e => e.ImagePath)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .HasColumnName("image_path");
                entity.Property(e => e.OldPrice)
                    .HasColumnType("decimal(18,2)")
                    .HasColumnName("old_price");
                entity.Property(e => e.CurrentPrice)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)")
                    .HasColumnName("current_price");
                entity.Property(e => e.InStock)
                    .IsRequired()
                    .HasColumnName("in_stock");
                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id");

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(e => e.CategoryId)
                    .HasConstraintName("product_category_id_fkey")
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.Link)
                    .IsUnique()
                    .HasDatabaseName("idx_product_link_unique");
                entity.HasIndex(e => e.Name)
                    .IsUnique()
                    .HasDatabaseName("idx_product_name_unique");
                entity.HasIndex(e => e.CategoryId)
                    .HasDatabaseName("idx_product_category_id_fkey");
            });
        }
    }
}
