using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddPrescriptionLabTestToReceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PrescriptionLabTestId",
                table: "LabTestReceipts",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabTestReceipts_PrescriptionLabTestId",
                table: "LabTestReceipts",
                column: "PrescriptionLabTestId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestReceipts_PrescriptionLabTests_PrescriptionLabTestId",
                table: "LabTestReceipts",
                column: "PrescriptionLabTestId",
                principalTable: "PrescriptionLabTests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabTestReceipts_PrescriptionLabTests_PrescriptionLabTestId",
                table: "LabTestReceipts");

            migrationBuilder.DropIndex(
                name: "IX_LabTestReceipts_PrescriptionLabTestId",
                table: "LabTestReceipts");

            migrationBuilder.DropColumn(
                name: "PrescriptionLabTestId",
                table: "LabTestReceipts");
        }
    }
}
