using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class addPatientDischarge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientDischarge",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    AdmissionId = table.Column<long>(type: "bigint", nullable: false),
                    PatientId = table.Column<long>(type: "bigint", nullable: false),
                    DischargeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DischargeSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DischargeStatus = table.Column<int>(type: "int", nullable: false),
                    ApprovedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientDischarge", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientDischarge_Admissions_AdmissionId",
                        column: x => x.AdmissionId,
                        principalTable: "Admissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientDischarge_Doctors_AdmissionId",
                        column: x => x.AdmissionId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientDischarge_Patients_AdmissionId",
                        column: x => x.AdmissionId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientDischarge_AdmissionId",
                table: "PatientDischarge",
                column: "AdmissionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientDischarge");
        }
    }
}
