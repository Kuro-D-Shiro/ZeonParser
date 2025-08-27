using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZeonService.Migrations
{
    /// <inheritdoc />
    public partial class New_Indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "idx_product_name",
                table: "products",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "idx_category_name",
                table: "categories",
                column: "name");
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
        }
    }
}
