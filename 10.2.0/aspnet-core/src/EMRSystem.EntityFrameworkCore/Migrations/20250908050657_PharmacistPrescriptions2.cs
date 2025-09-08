using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class PharmacistPrescriptions2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PharmacistPrescriptions_PrescriptionId",
                table: "PharmacistPrescriptions");

            migrationBuilder.CreateIndex(
                name: "IX_PharmacistPrescriptions_PrescriptionId",
                table: "PharmacistPrescriptions",
                column: "PrescriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PharmacistPrescriptions_PrescriptionId",
                table: "PharmacistPrescriptions");

            migrationBuilder.CreateIndex(
                name: "IX_PharmacistPrescriptions_PrescriptionId",
                table: "PharmacistPrescriptions",
                column: "PrescriptionId",
                unique: true,
                filter: "[PrescriptionId] IS NOT NULL");
        }
    }
}
