using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class Added_LabReportTypeItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LabReportTypeItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    LabReportTypeId = table.Column<long>(type: "bigint", nullable: false),
                    LabTestId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabReportTypeItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabReportTypeItems_LabReportsTypes_LabReportTypeId",
                        column: x => x.LabReportTypeId,
                        principalTable: "LabReportsTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabReportTypeItems_LabTests_LabTestId",
                        column: x => x.LabTestId,
                        principalTable: "LabTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabReportTypeItems_LabReportTypeId",
                table: "LabReportTypeItems",
                column: "LabReportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LabReportTypeItems_LabTestId",
                table: "LabReportTypeItems",
                column: "LabTestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabReportTypeItems");
        }
    }
}
