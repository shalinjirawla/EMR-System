using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class changedConsultationRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultationRequests_Patients_PatientId",
                table: "ConsultationRequests");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "ConsultationRequests",
                newName: "PrescriptionId");

            migrationBuilder.RenameIndex(
                name: "IX_ConsultationRequests_PatientId",
                table: "ConsultationRequests",
                newName: "IX_ConsultationRequests_PrescriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultationRequests_Prescriptions_PrescriptionId",
                table: "ConsultationRequests",
                column: "PrescriptionId",
                principalTable: "Prescriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultationRequests_Prescriptions_PrescriptionId",
                table: "ConsultationRequests");

            migrationBuilder.RenameColumn(
                name: "PrescriptionId",
                table: "ConsultationRequests",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_ConsultationRequests_PrescriptionId",
                table: "ConsultationRequests",
                newName: "IX_ConsultationRequests_PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultationRequests_Patients_PatientId",
                table: "ConsultationRequests",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
