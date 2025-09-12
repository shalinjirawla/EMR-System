using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class renameTablePatientDischarge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientDischarge_Admissions_AdmissionId",
                table: "PatientDischarge");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientDischarge_Doctors_AdmissionId",
                table: "PatientDischarge");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientDischarge_Patients_AdmissionId",
                table: "PatientDischarge");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PatientDischarge",
                table: "PatientDischarge");

            migrationBuilder.RenameTable(
                name: "PatientDischarge",
                newName: "PatientDischarges");

            migrationBuilder.RenameIndex(
                name: "IX_PatientDischarge_AdmissionId",
                table: "PatientDischarges",
                newName: "IX_PatientDischarges_AdmissionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PatientDischarges",
                table: "PatientDischarges",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientDischarges_Admissions_AdmissionId",
                table: "PatientDischarges",
                column: "AdmissionId",
                principalTable: "Admissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientDischarges_Admissions_AdmissionId",
                table: "PatientDischarges");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientDischarges_Doctors_AdmissionId",
                table: "PatientDischarges");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientDischarges_Patients_AdmissionId",
                table: "PatientDischarges");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PatientDischarges",
                table: "PatientDischarges");

            migrationBuilder.RenameTable(
                name: "PatientDischarges",
                newName: "PatientDischarge");

            migrationBuilder.RenameIndex(
                name: "IX_PatientDischarges_AdmissionId",
                table: "PatientDischarge",
                newName: "IX_PatientDischarge_AdmissionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PatientDischarge",
                table: "PatientDischarge",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientDischarge_Admissions_AdmissionId",
                table: "PatientDischarge",
                column: "AdmissionId",
                principalTable: "Admissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientDischarge_Doctors_AdmissionId",
                table: "PatientDischarge",
                column: "AdmissionId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientDischarge_Patients_AdmissionId",
                table: "PatientDischarge",
                column: "AdmissionId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
