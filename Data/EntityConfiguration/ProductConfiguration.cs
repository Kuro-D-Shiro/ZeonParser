using AngleSharp.Dom;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using ZeonService.Models;

namespace ZeonService.Data.EntityConfiguration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> entityBulder)
        {
            entityBulder.ToTable("products");
            entityBulder.ToTable(e => e.HasCheckConstraint(
                "ck_product_current_price",
                "current_price > 0"
            ));
            entityBulder.ToTable(e => e.HasCheckConstraint(
                "ck_product_price_logic",
                "old_price is null or old_price > current_price"
            ));

            entityBulder.HasKey(e => e.ProductId)
                .HasName("product_pkey");
            entityBulder.HasAlternateKey(e => e.Link)
                .HasName("product_link_key");

            entityBulder.Property(e => e.ProductId)
                .HasColumnName("product_id");
            entityBulder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("name");
            entityBulder.Property(e => e.Link)
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnName("link");
            entityBulder.Property(e => e.ImagePath)
                .IsRequired()
                .HasMaxLength(204)
                .HasColumnName("image_path");
            entityBulder.Property(e => e.OldPrice)
                .HasColumnType("decimal(18,2)")
                .HasColumnName("old_price");
            entityBulder.Property(e => e.CurrentPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasColumnName("current_price");
            entityBulder.Property(e => e.Specifications)
                .HasColumnName("specifications")
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(v, (JsonSerializerOptions)null)
                );
            entityBulder.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasColumnName("updated_at");
            entityBulder.Property(e => e.CategoryId)
                .HasColumnName("category_id");

            entityBulder.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryId)
                .HasConstraintName("product_category_id_fkey")
                .OnDelete(DeleteBehavior.Restrict);

            entityBulder.HasIndex(e => e.CategoryId)
                .HasDatabaseName("idx_product_category_id_fkey");
            entityBulder.HasIndex(e => e.Name)
                .HasDatabaseName("idx_product_name");
        }
    }
}
