using Microsoft.EntityFrameworkCore.Migrations;

namespace Pellio.Migrations
{
    public partial class UpdatedCart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CartId1",
                table: "Products",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CartId1",
                table: "Products",
                column: "CartId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Cart_CartId1",
                table: "Products",
                column: "CartId1",
                principalTable: "Cart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Cart_CartId1",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CartId1",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CartId1",
                table: "Products");
        }
    }
}
