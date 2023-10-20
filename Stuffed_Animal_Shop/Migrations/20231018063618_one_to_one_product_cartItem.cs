using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stuffed_Animal_Shop.Migrations
{
    /// <inheritdoc />
    public partial class one_to_one_product_cartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Products_CartItemId",
                table: "CartItems",
                column: "CartItemId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Products_CartItemId",
                table: "CartItems");
        }
    }
}
