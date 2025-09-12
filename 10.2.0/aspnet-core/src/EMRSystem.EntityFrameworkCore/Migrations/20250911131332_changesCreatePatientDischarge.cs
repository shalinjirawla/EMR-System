using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class changesCreatePatientDischarge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientDischarges_Doctors_AdmissionId",
                table: "PatientDischarges");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientDischarges_Patients_AdmissionId",
                table: "PatientDischarges");

            migrationBuilder.RenameColumn(
                name: "ApprovedBy",
                table: "PatientDischarges",
                newName: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDischarges_DoctorId",
                table: "PatientDischarges",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDischarges_PatientId",
                table: "PatientDischarges",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientDischarges_Doctors_DoctorId",
                table: "PatientDischarges",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientDischarges_Patients_PatientId",
                table: "PatientDischarges",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientDischarges_Doctors_DoctorId",
                table: "PatientDischarges");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientDischarges_Patients_PatientId",
                table: "PatientDischarges");

            migrationBuilder.DropIndex(
                name: "IX_PatientDischarges_DoctorId",
                table: "PatientDischarges");

            migrationBuilder.DropIndex(
                name: "IX_PatientDischarges_PatientId",
                table: "PatientDischarges");

            migrationBuilder.RenameColumn(
                name: "DoctorId",
                table: "PatientDischarges",
                newName: "ApprovedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientDischarges_Doctors_AdmissionId",
                table: "PatientDischarges",
                column: "AdmissionId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientDischarges_Patients_AdmissionId",
                table: "PatientDischarges",
                column: "AdmissionId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
