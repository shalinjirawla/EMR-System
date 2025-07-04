using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class addvistits2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visit_Departments_DepartmentId",
                table: "Visit");

            migrationBuilder.DropForeignKey(
                name: "FK_Visit_Doctors_DoctorId",
                table: "Visit");

            migrationBuilder.DropForeignKey(
                name: "FK_Visit_Patients_PatientId",
                table: "Visit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Visit",
                table: "Visit");

            migrationBuilder.RenameTable(
                name: "Visit",
                newName: "Visits");

            migrationBuilder.RenameIndex(
                name: "IX_Visit_PatientId",
                table: "Visits",
                newName: "IX_Visits_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Visit_DoctorId",
                table: "Visits",
                newName: "IX_Visits_DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Visit_DepartmentId",
                table: "Visits",
                newName: "IX_Visits_DepartmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Visits",
                table: "Visits",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Departments_DepartmentId",
                table: "Visits",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Doctors_DoctorId",
                table: "Visits",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Patients_PatientId",
                table: "Visits",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Departments_DepartmentId",
                table: "Visits");

            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Doctors_DoctorId",
                table: "Visits");

            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Patients_PatientId",
                table: "Visits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Visits",
                table: "Visits");

            migrationBuilder.RenameTable(
                name: "Visits",
                newName: "Visit");

            migrationBuilder.RenameIndex(
                name: "IX_Visits_PatientId",
                table: "Visit",
                newName: "IX_Visit_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Visits_DoctorId",
                table: "Visit",
                newName: "IX_Visit_DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Visits_DepartmentId",
                table: "Visit",
                newName: "IX_Visit_DepartmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Visit",
                table: "Visit",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Visit_Departments_DepartmentId",
                table: "Visit",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Visit_Doctors_DoctorId",
                table: "Visit",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Visit_Patients_PatientId",
                table: "Visit",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");
        }
    }
}
