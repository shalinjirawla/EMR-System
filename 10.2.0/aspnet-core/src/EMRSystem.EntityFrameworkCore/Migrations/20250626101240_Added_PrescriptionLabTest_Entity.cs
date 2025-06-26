using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class Added_PrescriptionLabTest_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrescriptionLabTests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    PrescriptionId = table.Column<long>(type: "bigint", nullable: false),
                    LabReportsTypeId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionLabTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrescriptionLabTests_LabReportsTypes_LabReportsTypeId",
                        column: x => x.LabReportsTypeId,
                        principalTable: "LabReportsTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrescriptionLabTests_Prescriptions_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "Prescriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionLabTests_LabReportsTypeId",
                table: "PrescriptionLabTests",
                column: "LabReportsTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionLabTests_PrescriptionId",
                table: "PrescriptionLabTests",
                column: "PrescriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrescriptionLabTests");
        }
    }
}
