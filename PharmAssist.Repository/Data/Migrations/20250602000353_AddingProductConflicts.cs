using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmAssist.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingProductConflicts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Conflicts",
                table: "Products",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Conflicts",
                table: "Products");
        }
    }
}
