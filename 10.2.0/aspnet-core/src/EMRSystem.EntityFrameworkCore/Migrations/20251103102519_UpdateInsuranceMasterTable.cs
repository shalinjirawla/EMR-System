using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInsuranceMasterTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoversDoctorVisit",
                table: "InsuranceMasters");

            migrationBuilder.DropColumn(
                name: "CoversLabTests",
                table: "InsuranceMasters");

            migrationBuilder.DropColumn(
                name: "CoversMedicines",
                table: "InsuranceMasters");

            migrationBuilder.DropColumn(
                name: "CoversProcedures",
                table: "InsuranceMasters");

            migrationBuilder.DropColumn(
                name: "CoversRoomCharge",
                table: "InsuranceMasters");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CoversDoctorVisit",
                table: "InsuranceMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CoversLabTests",
                table: "InsuranceMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CoversMedicines",
                table: "InsuranceMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CoversProcedures",
                table: "InsuranceMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CoversRoomCharge",
                table: "InsuranceMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
