using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZeonService.Migrations
{
    /// <inheritdoc />
    public partial class Update_Product_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "in_stock",
                table: "products");

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "products",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddCheckConstraint(
                name: "ck_product_current_price",
                table: "products",
                sql: "current_price > 0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_product_price_logic",
                table: "products",
                sql: "old_price is null or old_price > current_price");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_product_current_price",
                table: "products");

            migrationBuilder.DropCheckConstraint(
                name: "ck_product_price_logic",
                table: "products");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "products");

            migrationBuilder.AddColumn<bool>(
                name: "in_stock",
                table: "products",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
