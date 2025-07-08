using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class addmedicineordertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MedicineOrders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    NurseId = table.Column<long>(type: "bigint", nullable: false),
                    PatientId = table.Column<long>(type: "bigint", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicineOrders_Nurses_NurseId",
                        column: x => x.NurseId,
                        principalTable: "Nurses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MedicineOrders_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MedicineOrderItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicineOrderId = table.Column<long>(type: "bigint", nullable: false),
                    MedicineId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Dosage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicineOrderItems_MedicineOrders_MedicineOrderId",
                        column: x => x.MedicineOrderId,
                        principalTable: "MedicineOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicineOrderItems_PharmacistInventory_MedicineId",
                        column: x => x.MedicineId,
                        principalTable: "PharmacistInventory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrderItems_MedicineId",
                table: "MedicineOrderItems",
                column: "MedicineId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrderItems_MedicineOrderId",
                table: "MedicineOrderItems",
                column: "MedicineOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrders_NurseId",
                table: "MedicineOrders",
                column: "NurseId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrders_PatientId",
                table: "MedicineOrders",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicineOrderItems");

            migrationBuilder.DropTable(
                name: "MedicineOrders");
        }
    }
}
