using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class addPharmacyStaffId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BillingStaffId",
                table: "PatientDischarges",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PharmacyStaffId",
                table: "PatientDischarges",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientDischarges_BillingStaffId",
                table: "PatientDischarges",
                column: "BillingStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDischarges_PharmacyStaffId",
                table: "PatientDischarges",
                column: "PharmacyStaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientDischarges_Billing_BillingStaffId",
                table: "PatientDischarges",
                column: "BillingStaffId",
                principalTable: "Billing",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientDischarges_Pharmacists_PharmacyStaffId",
                table: "PatientDischarges",
                column: "PharmacyStaffId",
                principalTable: "Pharmacists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientDischarges_Billing_BillingStaffId",
                table: "PatientDischarges");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientDischarges_Pharmacists_PharmacyStaffId",
                table: "PatientDischarges");

            migrationBuilder.DropIndex(
                name: "IX_PatientDischarges_BillingStaffId",
                table: "PatientDischarges");

            migrationBuilder.DropIndex(
                name: "IX_PatientDischarges_PharmacyStaffId",
                table: "PatientDischarges");

            migrationBuilder.DropColumn(
                name: "BillingStaffId",
                table: "PatientDischarges");

            migrationBuilder.DropColumn(
                name: "PharmacyStaffId",
                table: "PatientDischarges");
        }
    }
}
