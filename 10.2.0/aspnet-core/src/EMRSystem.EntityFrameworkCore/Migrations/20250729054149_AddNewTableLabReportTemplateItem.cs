using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTableLabReportTemplateItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LabReportTemplateItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Test = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Result = table.Column<decimal>(type: "decimal(18,2)", maxLength: 256, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    MinValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LabReportsTypeId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabReportTemplateItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabReportTemplateItems_LabReportsTypes_LabReportsTypeId",
                        column: x => x.LabReportsTypeId,
                        principalTable: "LabReportsTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabReportTemplateItems_LabReportsTypeId",
                table: "LabReportTemplateItems",
                column: "LabReportsTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LabReportTemplateItems_TenantId_LabReportsTypeId",
                table: "LabReportTemplateItems",
                columns: new[] { "TenantId", "LabReportsTypeId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabReportTemplateItems");
        }
    }
}
