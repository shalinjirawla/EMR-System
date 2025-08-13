using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class updatetablePrescriptionLabTest1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "HealthPackageId",
                table: "PrescriptionLabTests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFromPackage",
                table: "PrescriptionLabTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrescribed",
                table: "PrescriptionLabTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionLabTests_HealthPackageId",
                table: "PrescriptionLabTests",
                column: "HealthPackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionLabTests_HealthPackages_HealthPackageId",
                table: "PrescriptionLabTests",
                column: "HealthPackageId",
                principalTable: "HealthPackages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionLabTests_HealthPackages_HealthPackageId",
                table: "PrescriptionLabTests");

            migrationBuilder.DropIndex(
                name: "IX_PrescriptionLabTests_HealthPackageId",
                table: "PrescriptionLabTests");

            migrationBuilder.DropColumn(
                name: "HealthPackageId",
                table: "PrescriptionLabTests");

            migrationBuilder.DropColumn(
                name: "IsFromPackage",
                table: "PrescriptionLabTests");

            migrationBuilder.DropColumn(
                name: "IsPrescribed",
                table: "PrescriptionLabTests");
        }
    }
}
