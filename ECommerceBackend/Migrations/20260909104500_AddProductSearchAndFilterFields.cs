using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceBackend.Migrations;

public partial class AddProductSearchAndFilterFields : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Category",
            table: "Products",
            type: "varchar(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AddColumn<string>(
            name: "Brand",
            table: "Products",
            type: "varchar(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex(
            name: "IX_Products_Brand",
            table: "Products",
            column: "Brand");

        migrationBuilder.CreateIndex(
            name: "IX_Products_Category",
            table: "Products",
            column: "Category");

        migrationBuilder.CreateIndex(
            name: "IX_Products_Price",
            table: "Products",
            column: "Price");

        migrationBuilder.CreateIndex(
            name: "IX_Products_StockQuantity",
            table: "Products",
            column: "StockQuantity");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Products_Brand",
            table: "Products");

        migrationBuilder.DropIndex(
            name: "IX_Products_Category",
            table: "Products");

        migrationBuilder.DropIndex(
            name: "IX_Products_Price",
            table: "Products");

        migrationBuilder.DropIndex(
            name: "IX_Products_StockQuantity",
            table: "Products");

        migrationBuilder.DropColumn(
            name: "Brand",
            table: "Products");

        migrationBuilder.DropColumn(
            name: "Category",
            table: "Products");
    }
}
