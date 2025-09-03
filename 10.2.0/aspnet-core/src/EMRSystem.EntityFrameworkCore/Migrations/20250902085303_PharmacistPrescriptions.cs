using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class PharmacistPrescriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PharmacistPrescriptions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    PrescriptionId = table.Column<long>(type: "bigint", nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Order_Status = table.Column<int>(type: "int", nullable: false),
                    PharmacyNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CollectionStatus = table.Column<int>(type: "int", nullable: false),
                    PickedUpBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PharmacistPrescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PharmacistPrescriptions_Nurses_PickedUpBy",
                        column: x => x.PickedUpBy,
                        principalTable: "Nurses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PharmacistPrescriptions_Prescriptions_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "Prescriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PharmacistPrescriptions_PickedUpBy",
                table: "PharmacistPrescriptions",
                column: "PickedUpBy");

            migrationBuilder.CreateIndex(
                name: "IX_PharmacistPrescriptions_PrescriptionId",
                table: "PharmacistPrescriptions",
                column: "PrescriptionId",
                unique: true,
                filter: "[PrescriptionId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PharmacistPrescriptions");
        }
    }
}
