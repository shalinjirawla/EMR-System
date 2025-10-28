using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class Added_BirthRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BirthRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    MotherId = table.Column<long>(type: "bigint", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BirthTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BirthType = table.Column<int>(type: "int", nullable: false),
                    DeliveryType = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<long>(type: "bigint", nullable: true),
                    NurseId = table.Column<long>(type: "bigint", nullable: true),
                    IsStillBirth = table.Column<bool>(type: "bit", nullable: false),
                    FatherName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BirthRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BirthRecords_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BirthRecords_Nurses_NurseId",
                        column: x => x.NurseId,
                        principalTable: "Nurses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BirthRecords_Patients_MotherId",
                        column: x => x.MotherId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BirthRecords_DoctorId",
                table: "BirthRecords",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_BirthRecords_MotherId",
                table: "BirthRecords",
                column: "MotherId");

            migrationBuilder.CreateIndex(
                name: "IX_BirthRecords_NurseId",
                table: "BirthRecords",
                column: "NurseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BirthRecords");
        }
    }
}
