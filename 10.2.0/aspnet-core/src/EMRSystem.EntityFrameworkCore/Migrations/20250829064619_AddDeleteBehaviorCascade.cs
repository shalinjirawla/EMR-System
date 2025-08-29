using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddDeleteBehaviorCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SelectedEmergencyProcedures_EmergencyProcedures_EmergencyProcedureId",
                table: "SelectedEmergencyProcedures");

            migrationBuilder.DropForeignKey(
                name: "FK_SelectedEmergencyProcedures_Prescriptions_PrescriptionId",
                table: "SelectedEmergencyProcedures");

            migrationBuilder.AddForeignKey(
                name: "FK_SelectedEmergencyProcedures_EmergencyProcedures_EmergencyProcedureId",
                table: "SelectedEmergencyProcedures",
                column: "EmergencyProcedureId",
                principalTable: "EmergencyProcedures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SelectedEmergencyProcedures_Prescriptions_PrescriptionId",
                table: "SelectedEmergencyProcedures",
                column: "PrescriptionId",
                principalTable: "Prescriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SelectedEmergencyProcedures_EmergencyProcedures_EmergencyProcedureId",
                table: "SelectedEmergencyProcedures");

            migrationBuilder.DropForeignKey(
                name: "FK_SelectedEmergencyProcedures_Prescriptions_PrescriptionId",
                table: "SelectedEmergencyProcedures");

            migrationBuilder.AddForeignKey(
                name: "FK_SelectedEmergencyProcedures_EmergencyProcedures_EmergencyProcedureId",
                table: "SelectedEmergencyProcedures",
                column: "EmergencyProcedureId",
                principalTable: "EmergencyProcedures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SelectedEmergencyProcedures_Prescriptions_PrescriptionId",
                table: "SelectedEmergencyProcedures",
                column: "PrescriptionId",
                principalTable: "Prescriptions",
                principalColumn: "Id");
        }
    }
}
