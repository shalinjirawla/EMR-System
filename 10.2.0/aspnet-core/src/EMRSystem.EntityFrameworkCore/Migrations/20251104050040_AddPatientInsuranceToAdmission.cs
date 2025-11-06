using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientInsuranceToAdmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PatientInsuranceId",
                table: "Admissions",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_PatientInsuranceId",
                table: "Admissions",
                column: "PatientInsuranceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_PatientInsurances_PatientInsuranceId",
                table: "Admissions",
                column: "PatientInsuranceId",
                principalTable: "PatientInsurances",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_PatientInsurances_PatientInsuranceId",
                table: "Admissions");

            migrationBuilder.DropIndex(
                name: "IX_Admissions_PatientInsuranceId",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "PatientInsuranceId",
                table: "Admissions");
        }
    }
}
