using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLabTestRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabTestReceipts_LabReportsTypes_LabReportTypeId",
                table: "LabTestReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_LabTestReceipts_PrescriptionLabTests_PrescriptionLabTestId",
                table: "LabTestReceipts");

            migrationBuilder.DropIndex(
                name: "IX_LabTestReceipts_LabReportTypeId",
                table: "LabTestReceipts");

            migrationBuilder.DropColumn(
                name: "LabReportTypeId",
                table: "LabTestReceipts");

            migrationBuilder.RenameColumn(
                name: "PrescriptionLabTestId",
                table: "LabTestReceipts",
                newName: "LabReportsTypeId");

            migrationBuilder.RenameColumn(
                name: "LabTestFee",
                table: "LabTestReceipts",
                newName: "TotalFee");

            migrationBuilder.RenameIndex(
                name: "IX_LabTestReceipts_PrescriptionLabTestId",
                table: "LabTestReceipts",
                newName: "IX_LabTestReceipts_LabReportsTypeId");

            migrationBuilder.AddColumn<long>(
                name: "LabTestReceiptId",
                table: "PrescriptionLabTests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "LabTestReceipts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionLabTests_LabTestReceiptId",
                table: "PrescriptionLabTests",
                column: "LabTestReceiptId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestReceipts_LabReportsTypes_LabReportsTypeId",
                table: "LabTestReceipts",
                column: "LabReportsTypeId",
                principalTable: "LabReportsTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionLabTests_LabTestReceipts_LabTestReceiptId",
                table: "PrescriptionLabTests",
                column: "LabTestReceiptId",
                principalTable: "LabTestReceipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabTestReceipts_LabReportsTypes_LabReportsTypeId",
                table: "LabTestReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionLabTests_LabTestReceipts_LabTestReceiptId",
                table: "PrescriptionLabTests");

            migrationBuilder.DropIndex(
                name: "IX_PrescriptionLabTests_LabTestReceiptId",
                table: "PrescriptionLabTests");

            migrationBuilder.DropColumn(
                name: "LabTestReceiptId",
                table: "PrescriptionLabTests");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "LabTestReceipts");

            migrationBuilder.RenameColumn(
                name: "TotalFee",
                table: "LabTestReceipts",
                newName: "LabTestFee");

            migrationBuilder.RenameColumn(
                name: "LabReportsTypeId",
                table: "LabTestReceipts",
                newName: "PrescriptionLabTestId");

            migrationBuilder.RenameIndex(
                name: "IX_LabTestReceipts_LabReportsTypeId",
                table: "LabTestReceipts",
                newName: "IX_LabTestReceipts_PrescriptionLabTestId");

            migrationBuilder.AddColumn<long>(
                name: "LabReportTypeId",
                table: "LabTestReceipts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_LabTestReceipts_LabReportTypeId",
                table: "LabTestReceipts",
                column: "LabReportTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestReceipts_LabReportsTypes_LabReportTypeId",
                table: "LabTestReceipts",
                column: "LabReportTypeId",
                principalTable: "LabReportsTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestReceipts_PrescriptionLabTests_PrescriptionLabTestId",
                table: "LabTestReceipts",
                column: "PrescriptionLabTestId",
                principalTable: "PrescriptionLabTests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
