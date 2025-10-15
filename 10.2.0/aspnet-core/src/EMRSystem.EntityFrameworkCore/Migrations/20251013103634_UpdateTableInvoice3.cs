using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableInvoice3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InsuranceClaims_PatientInsurances_PatientInsuranceId1",
                table: "InsuranceClaims");

            migrationBuilder.DropIndex(
                name: "IX_InsuranceClaims_PatientInsuranceId1",
                table: "InsuranceClaims");

            migrationBuilder.DropColumn(
                name: "PatientInsuranceId1",
                table: "InsuranceClaims");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PatientInsuranceId1",
                table: "InsuranceClaims",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_PatientInsuranceId1",
                table: "InsuranceClaims",
                column: "PatientInsuranceId1");

            migrationBuilder.AddForeignKey(
                name: "FK_InsuranceClaims_PatientInsurances_PatientInsuranceId1",
                table: "InsuranceClaims",
                column: "PatientInsuranceId1",
                principalTable: "PatientInsurances",
                principalColumn: "Id");
        }
    }
}
