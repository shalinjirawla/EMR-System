using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomFacilityAndTypeMasters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoomFacilityMasters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    FacilityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomFacilityMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoomTypeMasters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    TypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DefaultPricePerDay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomTypeMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoomTypeFacilities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    RoomTypeMasterId = table.Column<long>(type: "bigint", nullable: false),
                    RoomFacilityMasterId = table.Column<long>(type: "bigint", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomTypeFacilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomTypeFacilities_RoomFacilityMasters_RoomFacilityMasterId",
                        column: x => x.RoomFacilityMasterId,
                        principalTable: "RoomFacilityMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomTypeFacilities_RoomTypeMasters_RoomTypeMasterId",
                        column: x => x.RoomTypeMasterId,
                        principalTable: "RoomTypeMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomFacilityMasters_TenantId_FacilityName",
                table: "RoomFacilityMasters",
                columns: new[] { "TenantId", "FacilityName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomTypeFacilities_RoomFacilityMasterId",
                table: "RoomTypeFacilities",
                column: "RoomFacilityMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomTypeFacilities_RoomTypeMasterId",
                table: "RoomTypeFacilities",
                column: "RoomTypeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomTypeFacilities_TenantId_RoomTypeMasterId_RoomFacilityMasterId",
                table: "RoomTypeFacilities",
                columns: new[] { "TenantId", "RoomTypeMasterId", "RoomFacilityMasterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomTypeMasters_TenantId_TypeName",
                table: "RoomTypeMasters",
                columns: new[] { "TenantId", "TypeName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomTypeFacilities");

            migrationBuilder.DropTable(
                name: "RoomFacilityMasters");

            migrationBuilder.DropTable(
                name: "RoomTypeMasters");
        }
    }
}
