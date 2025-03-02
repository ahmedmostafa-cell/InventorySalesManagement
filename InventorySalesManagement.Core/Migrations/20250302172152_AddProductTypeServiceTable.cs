using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventorySalesManagement.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTypeServiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductType",
                table: "Services",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductType",
                table: "Services");
        }
    }
}
