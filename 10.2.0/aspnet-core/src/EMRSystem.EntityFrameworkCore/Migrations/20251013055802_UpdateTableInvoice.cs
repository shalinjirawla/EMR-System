using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ApprovedAmount",
                table: "Invoices",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CoPayAmount",
                table: "Invoices",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "InsuranceClaimId",
                table: "Invoices",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "InsuranceClaimId1",
                table: "Invoices",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsClaimGenerated",
                table: "Invoices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ApprovedAmount",
                table: "InvoiceItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCoveredByInsurance",
                table: "InvoiceItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "NotApprovedAmount",
                table: "InvoiceItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "InsuranceClaims",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InsuranceClaimId1",
                table: "Invoices",
                column: "InsuranceClaimId1");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_TenantId_InvoiceId",
                table: "InsuranceClaims",
                columns: new[] { "TenantId", "InvoiceId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_InsuranceClaims_InsuranceClaimId1",
                table: "Invoices",
                column: "InsuranceClaimId1",
                principalTable: "InsuranceClaims",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_InsuranceClaims_InsuranceClaimId1",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_InsuranceClaimId1",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_InsuranceClaims_TenantId_InvoiceId",
                table: "InsuranceClaims");

            migrationBuilder.DropColumn(
                name: "ApprovedAmount",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CoPayAmount",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "InsuranceClaimId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "InsuranceClaimId1",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "IsClaimGenerated",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ApprovedAmount",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "IsCoveredByInsurance",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "NotApprovedAmount",
                table: "InvoiceItems");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "InsuranceClaims",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);
        }
    }
}
