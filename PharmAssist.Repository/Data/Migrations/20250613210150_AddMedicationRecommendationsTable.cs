using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmAssist.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicationRecommendationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MedicationRecommendations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    SafetyScore = table.Column<double>(type: "float(3)", precision: 3, scale: 2, nullable: false),
                    EffectivenessScore = table.Column<double>(type: "float(3)", precision: 3, scale: 2, nullable: false),
                    ValueScore = table.Column<double>(type: "float(3)", precision: 3, scale: 2, nullable: false),
                    FinalScore = table.Column<double>(type: "float(3)", precision: 3, scale: 2, nullable: false),
                    HasConflict = table.Column<bool>(type: "bit", nullable: false),
                    ConflictDetails = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RecommendationReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationRecommendations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicationRecommendations_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicationRecommendations_CreatedAt",
                table: "MedicationRecommendations",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationRecommendations_ProductId",
                table: "MedicationRecommendations",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationRecommendations_UserId",
                table: "MedicationRecommendations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationRecommendations_UserId_HasConflict",
                table: "MedicationRecommendations",
                columns: new[] { "UserId", "HasConflict" });

            migrationBuilder.CreateIndex(
                name: "IX_MedicationRecommendations_UserId_IsActive",
                table: "MedicationRecommendations",
                columns: new[] { "UserId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicationRecommendations");
        }
    }
}
