using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZeonService.Migrations
{
    /// <inheritdoc />
    public partial class Update_Product_Add_Specifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "products");

            migrationBuilder.AddColumn<string>(
                name: "specifications",
                table: "products",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "specifications",
                table: "products");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "products",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
