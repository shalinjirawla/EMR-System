using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class updatetabledeposit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Admissions_AdmissionId",
                table: "Deposits");

            migrationBuilder.DropColumn(
                name: "IsDeposit",
                table: "Deposits");

            migrationBuilder.RenameColumn(
                name: "AdmissionId",
                table: "Deposits",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Deposits_AdmissionId",
                table: "Deposits",
                newName: "IX_Deposits_PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Patients_PatientId",
                table: "Deposits",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Patients_PatientId",
                table: "Deposits");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "Deposits",
                newName: "AdmissionId");

            migrationBuilder.RenameIndex(
                name: "IX_Deposits_PatientId",
                table: "Deposits",
                newName: "IX_Deposits_AdmissionId");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeposit",
                table: "Deposits",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Admissions_AdmissionId",
                table: "Deposits",
                column: "AdmissionId",
                principalTable: "Admissions",
                principalColumn: "Id");
        }
    }
}
