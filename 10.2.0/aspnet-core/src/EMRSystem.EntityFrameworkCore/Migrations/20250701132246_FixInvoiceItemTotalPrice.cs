using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class FixInvoiceItemTotalPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PrescriptionItemId",
                table: "InvoiceItems",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PrescriptionLabTestId",
                table: "InvoiceItems",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ServiceDate",
                table: "InvoiceItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "InvoiceItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_PrescriptionItemId",
                table: "InvoiceItems",
                column: "PrescriptionItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_PrescriptionLabTestId",
                table: "InvoiceItems",
                column: "PrescriptionLabTestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InvoiceItems_PrescriptionItemId",
                table: "InvoiceItems");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceItems_PrescriptionLabTestId",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "PrescriptionItemId",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "PrescriptionLabTestId",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "ServiceDate",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "InvoiceItems");
        }
    }
}
