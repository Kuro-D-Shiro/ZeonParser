using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZeonService.Models;

namespace ZeonService.Data.EntityConfiguration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> entityBuilder)
        {
            entityBuilder.ToTable("categories");

            entityBuilder.HasKey(e => e.CategoryId)
                .HasName("category_pkey");
            entityBuilder.HasAlternateKey(e => e.Link)
                .HasName("category_link_key");

            entityBuilder.Property(e => e.CategoryId)
                .HasColumnName("category_id");
            entityBuilder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("name");
            entityBuilder.Property(e => e.Link)
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnName("link");
            entityBuilder.Property(e => e.ParentCategoryId)
                .HasColumnName("parent_category_id");

            entityBuilder.HasOne(e => e.ParentCategory)
                .WithMany(p => p.ChildCategories)
                .HasForeignKey(e => e.ParentCategoryId)
                .HasConstraintName("category_parent_category_id_fkey");

            entityBuilder.HasMany(e => e.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .HasConstraintName("product_category_id_fkey")
                .OnDelete(DeleteBehavior.Restrict);

            entityBuilder.HasIndex(e => e.ParentCategoryId)
                .HasDatabaseName("idx_category_parent_category_id_fkey");
            entityBuilder.HasIndex(e => e.Name)
                .HasDatabaseName("idx_category_name");
        }
    }
}
