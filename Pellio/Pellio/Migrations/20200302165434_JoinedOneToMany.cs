using Microsoft.EntityFrameworkCore.Migrations;

namespace Pellio.Migrations
{
    public partial class JoinedOneToMany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductsId",
                table: "Comments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ProductsId",
                table: "Comments",
                column: "ProductsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Products_ProductsId",
                table: "Comments",
                column: "ProductsId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Products_ProductsId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ProductsId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ProductsId",
                table: "Comments");
        }
    }
}
