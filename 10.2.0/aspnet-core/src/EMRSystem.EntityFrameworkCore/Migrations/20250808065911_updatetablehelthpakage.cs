using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class updatetablehelthpakage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "HealthPackages",
                newName: "PackageName");

            migrationBuilder.AddColumn<decimal>(
                name: "PackagePrice",
                table: "HealthPackages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PackagePrice",
                table: "HealthPackages");

            migrationBuilder.RenameColumn(
                name: "PackageName",
                table: "HealthPackages",
                newName: "Name");
        }
    }
}
