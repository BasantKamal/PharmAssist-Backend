using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmAssist.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProductIdToBasketItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "BasketItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "BasketItems");
        }
    }
}
