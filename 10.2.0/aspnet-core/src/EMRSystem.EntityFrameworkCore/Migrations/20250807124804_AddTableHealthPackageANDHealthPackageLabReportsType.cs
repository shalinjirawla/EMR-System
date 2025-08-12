using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddTableHealthPackageANDHealthPackageLabReportsType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HealthPackages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthPackages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HealthPackageLabReportsTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    HealthPackageId = table.Column<long>(type: "bigint", nullable: false),
                    LabReportsTypeId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthPackageLabReportsTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthPackageLabReportsTypes_HealthPackages_HealthPackageId",
                        column: x => x.HealthPackageId,
                        principalTable: "HealthPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HealthPackageLabReportsTypes_LabReportsTypes_LabReportsTypeId",
                        column: x => x.LabReportsTypeId,
                        principalTable: "LabReportsTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HealthPackageLabReportsTypes_HealthPackageId",
                table: "HealthPackageLabReportsTypes",
                column: "HealthPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthPackageLabReportsTypes_LabReportsTypeId",
                table: "HealthPackageLabReportsTypes",
                column: "LabReportsTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HealthPackageLabReportsTypes");

            migrationBuilder.DropTable(
                name: "HealthPackages");
        }
    }
}
