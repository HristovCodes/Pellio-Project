using Microsoft.EntityFrameworkCore.Migrations;

namespace Pellio.Migrations
{
    public partial class GayAf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Comments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductsId",
                table: "Comments",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductsId1",
                table: "Comments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ProductsId",
                table: "Comments",
                column: "ProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ProductsId1",
                table: "Comments",
                column: "ProductsId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Products_ProductsId",
                table: "Comments",
                column: "ProductsId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Products_ProductsId1",
                table: "Comments",
                column: "ProductsId1",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Products_ProductsId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Products_ProductsId1",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ProductsId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ProductsId1",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ProductsId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ProductsId1",
                table: "Comments");

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
