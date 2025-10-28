using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class Added_DeathRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "BirthRecords",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "DeathRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<long>(type: "bigint", nullable: false),
                    DeathDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeathTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DoctorId = table.Column<long>(type: "bigint", nullable: true),
                    NurseId = table.Column<long>(type: "bigint", nullable: true),
                    CauseOfDeath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPostMortemDone = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeathRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeathRecords_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DeathRecords_Nurses_NurseId",
                        column: x => x.NurseId,
                        principalTable: "Nurses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DeathRecords_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeathRecords_DoctorId",
                table: "DeathRecords",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_DeathRecords_NurseId",
                table: "DeathRecords",
                column: "NurseId");

            migrationBuilder.CreateIndex(
                name: "IX_DeathRecords_PatientId",
                table: "DeathRecords",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeathRecords");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "BirthRecords",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
