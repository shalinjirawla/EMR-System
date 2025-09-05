using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class changePrescriptionItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrescribe",
                table: "PrescriptionItem",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "PrescriptionItem",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "PharmacistPrescriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrescribe",
                table: "PrescriptionItem");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "PrescriptionItem");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "PharmacistPrescriptions");
        }
    }
}
