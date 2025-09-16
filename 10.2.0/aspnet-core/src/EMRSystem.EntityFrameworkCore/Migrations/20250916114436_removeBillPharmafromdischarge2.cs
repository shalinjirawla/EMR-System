using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class removeBillPharmafromdischarge2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientDischarges_Billing_BillId",
                table: "PatientDischarges");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientDischarges_Pharmacists_PharmacistId",
                table: "PatientDischarges");

            migrationBuilder.DropIndex(
                name: "IX_PatientDischarges_BillId",
                table: "PatientDischarges");

            migrationBuilder.DropIndex(
                name: "IX_PatientDischarges_PharmacistId",
                table: "PatientDischarges");

            migrationBuilder.DropColumn(
                name: "BillId",
                table: "PatientDischarges");

            migrationBuilder.DropColumn(
                name: "PharmacistId",
                table: "PatientDischarges");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BillId",
                table: "PatientDischarges",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PharmacistId",
                table: "PatientDischarges",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientDischarges_BillId",
                table: "PatientDischarges",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDischarges_PharmacistId",
                table: "PatientDischarges",
                column: "PharmacistId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientDischarges_Billing_BillId",
                table: "PatientDischarges",
                column: "BillId",
                principalTable: "Billing",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientDischarges_Pharmacists_PharmacistId",
                table: "PatientDischarges",
                column: "PharmacistId",
                principalTable: "Pharmacists",
                principalColumn: "Id");
        }
    }
}
