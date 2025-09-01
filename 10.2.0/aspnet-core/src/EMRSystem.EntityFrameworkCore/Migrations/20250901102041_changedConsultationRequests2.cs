using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class changedConsultationRequests2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ConsultationRequests_PrescriptionId",
                table: "ConsultationRequests");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationRequests_PrescriptionId",
                table: "ConsultationRequests",
                column: "PrescriptionId",
                unique: true,
                filter: "[PrescriptionId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ConsultationRequests_PrescriptionId",
                table: "ConsultationRequests");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationRequests_PrescriptionId",
                table: "ConsultationRequests",
                column: "PrescriptionId");
        }
    }
}
