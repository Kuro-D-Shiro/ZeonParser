using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZeonService.Migrations
{
    /// <inheritdoc />
    public partial class Change_Products : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "product_name_key",
                table: "products");

            migrationBuilder.DropUniqueConstraint(
                name: "category_name_key",
                table: "categories");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "products",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddUniqueConstraint(
                name: "product_name_description_key",
                table: "products",
                columns: new[] { "name", "description" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "product_name_description_key",
                table: "products");

            migrationBuilder.DropColumn(
                name: "description",
                table: "products");

            migrationBuilder.AddUniqueConstraint(
                name: "product_name_key",
                table: "products",
                column: "name");

            migrationBuilder.AddUniqueConstraint(
                name: "category_name_key",
                table: "categories",
                column: "name");
        }
    }
}
