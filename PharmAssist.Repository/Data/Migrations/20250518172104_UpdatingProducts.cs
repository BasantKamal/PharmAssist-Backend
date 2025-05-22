using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmAssist.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActiveIngredient",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveIngredient",
                table: "Products");
        }
    }
}
