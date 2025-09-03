using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class PharmacistPrescriptionsItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PharmacistPrescriptionsItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    PharmacistPrescriptionId = table.Column<long>(type: "bigint", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PharmacistPrescriptionsItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PharmacistPrescriptionsItem_PharmacistPrescriptions_PharmacistPrescriptionId",
                        column: x => x.PharmacistPrescriptionId,
                        principalTable: "PharmacistPrescriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PharmacistPrescriptionsItem_PharmacistPrescriptionId",
                table: "PharmacistPrescriptionsItem",
                column: "PharmacistPrescriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PharmacistPrescriptionsItem");
        }
    }
}
