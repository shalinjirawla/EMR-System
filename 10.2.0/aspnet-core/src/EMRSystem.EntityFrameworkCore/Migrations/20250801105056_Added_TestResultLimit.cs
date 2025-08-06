using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class Added_TestResultLimit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "MeasureUnitId",
                table: "LabTests",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateTable(
                name: "TestResultLimits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    LabTestId = table.Column<long>(type: "bigint", nullable: false),
                    MinRange = table.Column<float>(type: "real", nullable: true),
                    MaxRange = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResultLimits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestResultLimits_LabTests_LabTestId",
                        column: x => x.LabTestId,
                        principalTable: "LabTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestResultLimits_LabTestId",
                table: "TestResultLimits",
                column: "LabTestId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultLimits_TenantId_LabTestId",
                table: "TestResultLimits",
                columns: new[] { "TenantId", "LabTestId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestResultLimits");

            migrationBuilder.AlterColumn<long>(
                name: "MeasureUnitId",
                table: "LabTests",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
