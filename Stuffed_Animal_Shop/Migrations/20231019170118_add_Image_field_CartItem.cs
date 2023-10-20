using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stuffed_Animal_Shop.Migrations
{
    /// <inheritdoc />
    public partial class add_Image_field_CartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "CartItems",
                type: "nvarchar(200)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "CartItems");
        }
    }
}
