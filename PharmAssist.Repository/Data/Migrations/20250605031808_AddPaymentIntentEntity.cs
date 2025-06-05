using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmAssist.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentIntentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentIntents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StripePaymentIntentId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "usd"),
                    BuyerEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    BasketId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClientSecret = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentIntents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentIntents_BasketId",
                table: "PaymentIntents",
                column: "BasketId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentIntents_StripePaymentIntentId",
                table: "PaymentIntents",
                column: "StripePaymentIntentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentIntents");
        }
    }
}
