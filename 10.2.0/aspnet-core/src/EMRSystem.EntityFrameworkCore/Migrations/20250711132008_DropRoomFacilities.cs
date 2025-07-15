using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class DropRoomFacilities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomFacilities");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "PricePerDay",
                table: "Rooms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "Rooms",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatorUserId",
                table: "Rooms",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "Rooms",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "Rooms",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Rooms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "Rooms",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastModifierUserId",
                table: "Rooms",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerDay",
                table: "Rooms",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "RoomFacilities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomFacilityMasterId = table.Column<long>(type: "bigint", nullable: false),
                    RoomId = table.Column<long>(type: "bigint", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomFacilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomFacilities_RoomFacilityMasters_RoomFacilityMasterId",
                        column: x => x.RoomFacilityMasterId,
                        principalTable: "RoomFacilityMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomFacilities_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomFacilities_RoomFacilityMasterId",
                table: "RoomFacilities",
                column: "RoomFacilityMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomFacilities_RoomId",
                table: "RoomFacilities",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomFacilities_TenantId_RoomId_RoomFacilityMasterId",
                table: "RoomFacilities",
                columns: new[] { "TenantId", "RoomId", "RoomFacilityMasterId" },
                unique: true);
        }
    }
}
