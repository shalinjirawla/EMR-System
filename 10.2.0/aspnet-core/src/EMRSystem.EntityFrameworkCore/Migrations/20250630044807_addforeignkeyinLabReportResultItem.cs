using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class addforeignkeyinLabReportResultItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PrescriptionLabTestId",
                table: "LabReportResultItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_LabReportResultItems_PrescriptionLabTestId",
                table: "LabReportResultItems",
                column: "PrescriptionLabTestId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabReportResultItems_PrescriptionLabTests_PrescriptionLabTestId",
                table: "LabReportResultItems",
                column: "PrescriptionLabTestId",
                principalTable: "PrescriptionLabTests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabReportResultItems_PrescriptionLabTests_PrescriptionLabTestId",
                table: "LabReportResultItems");

            migrationBuilder.DropIndex(
                name: "IX_LabReportResultItems_PrescriptionLabTestId",
                table: "LabReportResultItems");

            migrationBuilder.DropColumn(
                name: "PrescriptionLabTestId",
                table: "LabReportResultItems");
        }
    }
}
