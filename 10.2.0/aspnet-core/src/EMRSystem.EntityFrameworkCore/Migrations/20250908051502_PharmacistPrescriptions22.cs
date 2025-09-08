using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class PharmacistPrescriptions22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionItem_Prescriptions_PrescriptionId",
                table: "PrescriptionItem");

            migrationBuilder.AddColumn<long>(
                name: "PharmacistPrescriptionId",
                table: "PrescriptionItem",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionItem_PharmacistPrescriptionId",
                table: "PrescriptionItem",
                column: "PharmacistPrescriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionItem_PharmacistPrescriptions_PharmacistPrescriptionId",
                table: "PrescriptionItem",
                column: "PharmacistPrescriptionId",
                principalTable: "PharmacistPrescriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionItem_Prescriptions_PrescriptionId",
                table: "PrescriptionItem",
                column: "PrescriptionId",
                principalTable: "Prescriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionItem_PharmacistPrescriptions_PharmacistPrescriptionId",
                table: "PrescriptionItem");

            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionItem_Prescriptions_PrescriptionId",
                table: "PrescriptionItem");

            migrationBuilder.DropIndex(
                name: "IX_PrescriptionItem_PharmacistPrescriptionId",
                table: "PrescriptionItem");

            migrationBuilder.DropColumn(
                name: "PharmacistPrescriptionId",
                table: "PrescriptionItem");

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionItem_Prescriptions_PrescriptionId",
                table: "PrescriptionItem",
                column: "PrescriptionId",
                principalTable: "Prescriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
