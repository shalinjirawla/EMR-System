using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class removeBillPharmafromdischarge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientDischarges_Billing_BillingStaffId",
                table: "PatientDischarges");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientDischarges_Pharmacists_PharmacyStaffId",
                table: "PatientDischarges");

            migrationBuilder.RenameColumn(
                name: "PharmacyStaffId",
                table: "PatientDischarges",
                newName: "PharmacistId");

            migrationBuilder.RenameColumn(
                name: "BillingStaffId",
                table: "PatientDischarges",
                newName: "BillId");

            migrationBuilder.RenameIndex(
                name: "IX_PatientDischarges_PharmacyStaffId",
                table: "PatientDischarges",
                newName: "IX_PatientDischarges_PharmacistId");

            migrationBuilder.RenameIndex(
                name: "IX_PatientDischarges_BillingStaffId",
                table: "PatientDischarges",
                newName: "IX_PatientDischarges_BillId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientDischarges_Billing_BillId",
                table: "PatientDischarges");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientDischarges_Pharmacists_PharmacistId",
                table: "PatientDischarges");

            migrationBuilder.RenameColumn(
                name: "PharmacistId",
                table: "PatientDischarges",
                newName: "PharmacyStaffId");

            migrationBuilder.RenameColumn(
                name: "BillId",
                table: "PatientDischarges",
                newName: "BillingStaffId");

            migrationBuilder.RenameIndex(
                name: "IX_PatientDischarges_PharmacistId",
                table: "PatientDischarges",
                newName: "IX_PatientDischarges_PharmacyStaffId");

            migrationBuilder.RenameIndex(
                name: "IX_PatientDischarges_BillId",
                table: "PatientDischarges",
                newName: "IX_PatientDischarges_BillingStaffId");

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
    }
}
