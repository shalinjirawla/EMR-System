using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class updatetableIpdChargeEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PrescriptionId",
                table: "IpdChargeEntries",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IpdChargeEntries_PrescriptionId",
                table: "IpdChargeEntries",
                column: "PrescriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_IpdChargeEntries_Prescriptions_PrescriptionId",
                table: "IpdChargeEntries",
                column: "PrescriptionId",
                principalTable: "Prescriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IpdChargeEntries_Prescriptions_PrescriptionId",
                table: "IpdChargeEntries");

            migrationBuilder.DropIndex(
                name: "IX_IpdChargeEntries_PrescriptionId",
                table: "IpdChargeEntries");

            migrationBuilder.DropColumn(
                name: "PrescriptionId",
                table: "IpdChargeEntries");
        }
    }
}
