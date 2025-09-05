using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class removePharmacistPrescriptionsItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PharmacistPrescriptionsItem");

            migrationBuilder.AddColumn<decimal>(
                name: "GrandTotal",
                table: "PharmacistPrescriptions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrandTotal",
                table: "PharmacistPrescriptions");

            migrationBuilder.CreateTable(
                name: "PharmacistPrescriptionsItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PharmacistPrescriptionId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
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
    }
}
