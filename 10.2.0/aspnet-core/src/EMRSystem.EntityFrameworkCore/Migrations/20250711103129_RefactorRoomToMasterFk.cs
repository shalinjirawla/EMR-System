using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class RefactorRoomToMasterFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoomFacilities_TenantId_RoomId_Facility",
                table: "RoomFacilities");

            migrationBuilder.DropColumn(
                name: "RoomType",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "Facility",
                table: "RoomFacilities");

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

            migrationBuilder.AddColumn<long>(
                name: "RoomTypeMasterId",
                table: "Rooms",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "RoomFacilities",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatorUserId",
                table: "RoomFacilities",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RoomFacilityMasterId",
                table: "RoomFacilities",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomTypeMasterId",
                table: "Rooms",
                column: "RoomTypeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomFacilities_RoomFacilityMasterId",
                table: "RoomFacilities",
                column: "RoomFacilityMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomFacilities_TenantId_RoomId_RoomFacilityMasterId",
                table: "RoomFacilities",
                columns: new[] { "TenantId", "RoomId", "RoomFacilityMasterId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomFacilities_RoomFacilityMasters_RoomFacilityMasterId",
                table: "RoomFacilities",
                column: "RoomFacilityMasterId",
                principalTable: "RoomFacilityMasters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_RoomTypeMasters_RoomTypeMasterId",
                table: "Rooms",
                column: "RoomTypeMasterId",
                principalTable: "RoomTypeMasters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomFacilities_RoomFacilityMasters_RoomFacilityMasterId",
                table: "RoomFacilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_RoomTypeMasters_RoomTypeMasterId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_RoomTypeMasterId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_RoomFacilities_RoomFacilityMasterId",
                table: "RoomFacilities");

            migrationBuilder.DropIndex(
                name: "IX_RoomFacilities_TenantId_RoomId_RoomFacilityMasterId",
                table: "RoomFacilities");

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
                name: "IsDeleted",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "RoomTypeMasterId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "RoomFacilities");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "RoomFacilities");

            migrationBuilder.DropColumn(
                name: "RoomFacilityMasterId",
                table: "RoomFacilities");

            migrationBuilder.AddColumn<string>(
                name: "RoomType",
                table: "Rooms",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Facility",
                table: "RoomFacilities",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RoomFacilities_TenantId_RoomId_Facility",
                table: "RoomFacilities",
                columns: new[] { "TenantId", "RoomId", "Facility" },
                unique: true);
        }
    }
}
