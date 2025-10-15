using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableInvoice4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InsuranceClaims_Invoices_InvoiceId1",
                table: "InsuranceClaims");

            migrationBuilder.DropIndex(
                name: "IX_InsuranceClaims_InvoiceId1",
                table: "InsuranceClaims");

            migrationBuilder.DropColumn(
                name: "InvoiceId1",
                table: "InsuranceClaims");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "InvoiceId1",
                table: "InsuranceClaims",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_InvoiceId1",
                table: "InsuranceClaims",
                column: "InvoiceId1");

            migrationBuilder.AddForeignKey(
                name: "FK_InsuranceClaims_Invoices_InvoiceId1",
                table: "InsuranceClaims",
                column: "InvoiceId1",
                principalTable: "Invoices",
                principalColumn: "Id");
        }
    }
}
