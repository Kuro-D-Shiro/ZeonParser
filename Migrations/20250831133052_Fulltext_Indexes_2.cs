using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZeonService.Migrations
{
    /// <inheritdoc />
    public partial class Fulltext_Indexes_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_product_name",
                table: "products");

            migrationBuilder.DropIndex(
                name: "idx_category_name",
                table: "categories");

            migrationBuilder.CreateIndex(
                name: "idx_product_name",
                table: "products",
                column: "name")
                .Annotation("Npgsql:IndexMethod", "gist")
                .Annotation("Npgsql:IndexOperators", new[] { "gist_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "idx_category_name",
                table: "categories",
                column: "name")
                .Annotation("Npgsql:IndexMethod", "gist")
                .Annotation("Npgsql:IndexOperators", new[] { "gist_trgm_ops" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_product_name",
                table: "products");

            migrationBuilder.DropIndex(
                name: "idx_category_name",
                table: "categories");

            migrationBuilder.CreateIndex(
                name: "idx_product_name",
                table: "products",
                column: "name")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "idx_category_name",
                table: "categories",
                column: "name")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });
        }
    }
}
