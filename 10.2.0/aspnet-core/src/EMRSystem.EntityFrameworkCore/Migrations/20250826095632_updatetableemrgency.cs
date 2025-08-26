using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class updatetableemrgency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EmergencyCaseId",
                table: "EmergencyChargeEntry",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyChargeEntry_EmergencyCaseId",
                table: "EmergencyChargeEntry",
                column: "EmergencyCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyChargeEntry_PatientId",
                table: "EmergencyChargeEntry",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyChargeEntry_EmergencyCases_EmergencyCaseId",
                table: "EmergencyChargeEntry",
                column: "EmergencyCaseId",
                principalTable: "EmergencyCases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyChargeEntry_Patients_PatientId",
                table: "EmergencyChargeEntry",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyChargeEntry_EmergencyCases_EmergencyCaseId",
                table: "EmergencyChargeEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyChargeEntry_Patients_PatientId",
                table: "EmergencyChargeEntry");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyChargeEntry_EmergencyCaseId",
                table: "EmergencyChargeEntry");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyChargeEntry_PatientId",
                table: "EmergencyChargeEntry");

            migrationBuilder.DropColumn(
                name: "EmergencyCaseId",
                table: "EmergencyChargeEntry");
        }
    }
}
