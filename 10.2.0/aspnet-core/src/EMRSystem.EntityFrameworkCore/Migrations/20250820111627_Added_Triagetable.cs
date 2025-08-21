using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class Added_Triagetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EmergencyNumber",
                table: "EmergencyCases",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Triages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    EmergencyCaseId = table.Column<long>(type: "bigint", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Temperature = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Pulse = table.Column<int>(type: "int", nullable: true),
                    RespiratoryRate = table.Column<int>(type: "int", nullable: true),
                    BloodPressureSystolic = table.Column<int>(type: "int", nullable: true),
                    BloodPressureDiastolic = table.Column<int>(type: "int", nullable: true),
                    AssessmentTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Triages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Triages_EmergencyCases_EmergencyCaseId",
                        column: x => x.EmergencyCaseId,
                        principalTable: "EmergencyCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyCases_DoctorId",
                table: "EmergencyCases",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyCases_EmergencyNumber",
                table: "EmergencyCases",
                column: "EmergencyNumber",
                unique: true,
                filter: "[EmergencyNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyCases_NurseId",
                table: "EmergencyCases",
                column: "NurseId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyCases_PatientId",
                table: "EmergencyCases",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Triages_EmergencyCaseId",
                table: "Triages",
                column: "EmergencyCaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyCases_Doctors_DoctorId",
                table: "EmergencyCases",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyCases_Nurses_NurseId",
                table: "EmergencyCases",
                column: "NurseId",
                principalTable: "Nurses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyCases_Patients_PatientId",
                table: "EmergencyCases",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyCases_Doctors_DoctorId",
                table: "EmergencyCases");

            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyCases_Nurses_NurseId",
                table: "EmergencyCases");

            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyCases_Patients_PatientId",
                table: "EmergencyCases");

            migrationBuilder.DropTable(
                name: "Triages");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyCases_DoctorId",
                table: "EmergencyCases");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyCases_EmergencyNumber",
                table: "EmergencyCases");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyCases_NurseId",
                table: "EmergencyCases");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyCases_PatientId",
                table: "EmergencyCases");

            migrationBuilder.AlterColumn<string>(
                name: "EmergencyNumber",
                table: "EmergencyCases",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
