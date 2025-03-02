using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventorySalesManagement.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddQtyServiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImgUrl",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "MainSections");

            migrationBuilder.AddColumn<int>(
                name: "Qty",
                table: "Services",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Qty",
                table: "Services");

            migrationBuilder.AddColumn<string>(
                name: "ImgUrl",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "MainSections",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
