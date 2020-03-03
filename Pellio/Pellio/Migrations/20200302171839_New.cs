using Microsoft.EntityFrameworkCore.Migrations;

namespace Pellio.Migrations
{
    public partial class New : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Products_ProductsId",
                table: "Comments");

            migrationBuilder.AlterColumn<int>(
                name: "ProductsId",
                table: "Comments",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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

            migrationBuilder.AlterColumn<int>(
                name: "ProductsId",
                table: "Comments",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Products_ProductsId",
                table: "Comments",
                column: "ProductsId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
