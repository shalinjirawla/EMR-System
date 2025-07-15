using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class fixroommaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "RoomTypeMasters");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "RoomTypeMasters");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "RoomTypeMasters");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "RoomTypeMasters");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "RoomTypeMasters");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "RoomTypeMasters");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "RoomTypeMasters");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "RoomTypeFacilities");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "RoomTypeFacilities");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "RoomTypeMasters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatorUserId",
                table: "RoomTypeMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "RoomTypeMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "RoomTypeMasters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "RoomTypeMasters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "RoomTypeMasters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastModifierUserId",
                table: "RoomTypeMasters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "RoomTypeFacilities",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatorUserId",
                table: "RoomTypeFacilities",
                type: "bigint",
                nullable: true);
        }
    }
}
