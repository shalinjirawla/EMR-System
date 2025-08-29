using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class PrescriptionsEmergencyChargeEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PrescriptionId",
                table: "EmergencyChargeEntry",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyChargeEntry_PrescriptionId",
                table: "EmergencyChargeEntry",
                column: "PrescriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyChargeEntry_Prescriptions_PrescriptionId",
                table: "EmergencyChargeEntry",
                column: "PrescriptionId",
                principalTable: "Prescriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyChargeEntry_Prescriptions_PrescriptionId",
                table: "EmergencyChargeEntry");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyChargeEntry_PrescriptionId",
                table: "EmergencyChargeEntry");

            migrationBuilder.DropColumn(
                name: "PrescriptionId",
                table: "EmergencyChargeEntry");
        }
    }
}
