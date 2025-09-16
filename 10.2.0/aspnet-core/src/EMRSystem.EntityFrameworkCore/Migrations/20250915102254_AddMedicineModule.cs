using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicineModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MedicineFormMasters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineFormMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StrengthUnitMasters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StrengthUnitMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicineMasters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MedicineFormId = table.Column<long>(type: "bigint", nullable: false),
                    StrengthUnitId = table.Column<long>(type: "bigint", nullable: false),
                    MinimumStock = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineMasters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicineMasters_MedicineFormMasters_MedicineFormId",
                        column: x => x.MedicineFormId,
                        principalTable: "MedicineFormMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicineMasters_StrengthUnitMasters_StrengthUnitId",
                        column: x => x.StrengthUnitId,
                        principalTable: "StrengthUnitMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicineStocks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    MedicineMasterId = table.Column<long>(type: "bigint", nullable: false),
                    BatchNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsExpire = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicineStocks_MedicineMasters_MedicineMasterId",
                        column: x => x.MedicineMasterId,
                        principalTable: "MedicineMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicineFormMasters_TenantId_Name",
                table: "MedicineFormMasters",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicineMasters_MedicineFormId",
                table: "MedicineMasters",
                column: "MedicineFormId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineMasters_StrengthUnitId",
                table: "MedicineMasters",
                column: "StrengthUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineStocks_MedicineMasterId",
                table: "MedicineStocks",
                column: "MedicineMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_StrengthUnitMasters_TenantId_Name",
                table: "StrengthUnitMasters",
                columns: new[] { "TenantId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicineStocks");

            migrationBuilder.DropTable(
                name: "MedicineMasters");

            migrationBuilder.DropTable(
                name: "MedicineFormMasters");

            migrationBuilder.DropTable(
                name: "StrengthUnitMasters");
        }
    }
}
