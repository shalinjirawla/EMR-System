using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class changePrescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EmergencyCaseId",
                table: "Prescriptions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmergencyPrescription",
                table: "Prescriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "EmergencyCaseId",
                table: "PrescriptionLabTests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmergencyPrescription",
                table: "PrescriptionLabTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_EmergencyCaseId",
                table: "Prescriptions",
                column: "EmergencyCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionLabTests_EmergencyCaseId",
                table: "PrescriptionLabTests",
                column: "EmergencyCaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionLabTests_EmergencyCases_EmergencyCaseId",
                table: "PrescriptionLabTests",
                column: "EmergencyCaseId",
                principalTable: "EmergencyCases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescriptions_EmergencyCases_EmergencyCaseId",
                table: "Prescriptions",
                column: "EmergencyCaseId",
                principalTable: "EmergencyCases",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionLabTests_EmergencyCases_EmergencyCaseId",
                table: "PrescriptionLabTests");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescriptions_EmergencyCases_EmergencyCaseId",
                table: "Prescriptions");

            migrationBuilder.DropIndex(
                name: "IX_Prescriptions_EmergencyCaseId",
                table: "Prescriptions");

            migrationBuilder.DropIndex(
                name: "IX_PrescriptionLabTests_EmergencyCaseId",
                table: "PrescriptionLabTests");

            migrationBuilder.DropColumn(
                name: "EmergencyCaseId",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "IsEmergencyPrescription",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "EmergencyCaseId",
                table: "PrescriptionLabTests");

            migrationBuilder.DropColumn(
                name: "IsEmergencyPrescription",
                table: "PrescriptionLabTests");
        }
    }
}
