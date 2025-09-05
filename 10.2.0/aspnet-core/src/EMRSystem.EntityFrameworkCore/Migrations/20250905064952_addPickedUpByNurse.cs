using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class addPickedUpByNurse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PharmacistPrescriptions_Nurses_PickedUpBy",
                table: "PharmacistPrescriptions");

            migrationBuilder.RenameColumn(
                name: "PickedUpBy",
                table: "PharmacistPrescriptions",
                newName: "PickedUpByPatient");

            migrationBuilder.RenameIndex(
                name: "IX_PharmacistPrescriptions_PickedUpBy",
                table: "PharmacistPrescriptions",
                newName: "IX_PharmacistPrescriptions_PickedUpByPatient");

            migrationBuilder.AddColumn<long>(
                name: "PickedUpByNurse",
                table: "PharmacistPrescriptions",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PharmacistPrescriptions_PickedUpByNurse",
                table: "PharmacistPrescriptions",
                column: "PickedUpByNurse");

            migrationBuilder.AddForeignKey(
                name: "FK_PharmacistPrescriptions_Nurses_PickedUpByNurse",
                table: "PharmacistPrescriptions",
                column: "PickedUpByNurse",
                principalTable: "Nurses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PharmacistPrescriptions_Patients_PickedUpByPatient",
                table: "PharmacistPrescriptions",
                column: "PickedUpByPatient",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PharmacistPrescriptions_Nurses_PickedUpByNurse",
                table: "PharmacistPrescriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_PharmacistPrescriptions_Patients_PickedUpByPatient",
                table: "PharmacistPrescriptions");

            migrationBuilder.DropIndex(
                name: "IX_PharmacistPrescriptions_PickedUpByNurse",
                table: "PharmacistPrescriptions");

            migrationBuilder.DropColumn(
                name: "PickedUpByNurse",
                table: "PharmacistPrescriptions");

            migrationBuilder.RenameColumn(
                name: "PickedUpByPatient",
                table: "PharmacistPrescriptions",
                newName: "PickedUpBy");

            migrationBuilder.RenameIndex(
                name: "IX_PharmacistPrescriptions_PickedUpByPatient",
                table: "PharmacistPrescriptions",
                newName: "IX_PharmacistPrescriptions_PickedUpBy");

            migrationBuilder.AddForeignKey(
                name: "FK_PharmacistPrescriptions_Nurses_PickedUpBy",
                table: "PharmacistPrescriptions",
                column: "PickedUpBy",
                principalTable: "Nurses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
