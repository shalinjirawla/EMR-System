using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableInvoice2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_InsuranceClaims_InsuranceClaimId1",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_InsuranceClaimId1",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "InsuranceClaimId1",
                table: "Invoices");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "IpdChargeEntries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "InvoiceId1",
                table: "InsuranceClaims",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PatientInsuranceId1",
                table: "InsuranceClaims",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "EmergencyChargeEntry",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_InvoiceId1",
                table: "InsuranceClaims",
                column: "InvoiceId1");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_PatientInsuranceId1",
                table: "InsuranceClaims",
                column: "PatientInsuranceId1");

            migrationBuilder.AddForeignKey(
                name: "FK_InsuranceClaims_Invoices_InvoiceId1",
                table: "InsuranceClaims",
                column: "InvoiceId1",
                principalTable: "Invoices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InsuranceClaims_PatientInsurances_PatientInsuranceId1",
                table: "InsuranceClaims",
                column: "PatientInsuranceId1",
                principalTable: "PatientInsurances",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InsuranceClaims_Invoices_InvoiceId1",
                table: "InsuranceClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_InsuranceClaims_PatientInsurances_PatientInsuranceId1",
                table: "InsuranceClaims");

            migrationBuilder.DropIndex(
                name: "IX_InsuranceClaims_InvoiceId1",
                table: "InsuranceClaims");

            migrationBuilder.DropIndex(
                name: "IX_InsuranceClaims_PatientInsuranceId1",
                table: "InsuranceClaims");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "IpdChargeEntries");

            migrationBuilder.DropColumn(
                name: "InvoiceId1",
                table: "InsuranceClaims");

            migrationBuilder.DropColumn(
                name: "PatientInsuranceId1",
                table: "InsuranceClaims");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "EmergencyChargeEntry");

            migrationBuilder.AddColumn<long>(
                name: "InsuranceClaimId1",
                table: "Invoices",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InsuranceClaimId1",
                table: "Invoices",
                column: "InsuranceClaimId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_InsuranceClaims_InsuranceClaimId1",
                table: "Invoices",
                column: "InsuranceClaimId1",
                principalTable: "InsuranceClaims",
                principalColumn: "Id");
        }
    }
}
