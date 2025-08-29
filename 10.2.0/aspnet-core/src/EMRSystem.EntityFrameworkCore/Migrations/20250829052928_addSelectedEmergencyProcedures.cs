using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class addSelectedEmergencyProcedures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SelectedEmergencyProcedures",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    EmergencyProcedureId = table.Column<long>(type: "bigint", nullable: false),
                    PrescriptionId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectedEmergencyProcedures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SelectedEmergencyProcedures_EmergencyProcedures_EmergencyProcedureId",
                        column: x => x.EmergencyProcedureId,
                        principalTable: "EmergencyProcedures",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SelectedEmergencyProcedures_Prescriptions_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "Prescriptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SelectedEmergencyProcedures_EmergencyProcedureId",
                table: "SelectedEmergencyProcedures",
                column: "EmergencyProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedEmergencyProcedures_PrescriptionId",
                table: "SelectedEmergencyProcedures",
                column: "PrescriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SelectedEmergencyProcedures");
        }
    }
}
