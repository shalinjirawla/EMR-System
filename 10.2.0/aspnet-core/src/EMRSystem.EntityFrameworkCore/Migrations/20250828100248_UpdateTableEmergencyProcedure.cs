using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableEmergencyProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "EmergencyProcedures");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "EmergencyProcedures");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "EmergencyProcedures");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "EmergencyProcedures");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EmergencyProcedures");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "EmergencyProcedures");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "EmergencyProcedures");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "EmergencyProcedures",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Category",
                table: "EmergencyProcedures",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "EmergencyProcedures",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatorUserId",
                table: "EmergencyProcedures",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "EmergencyProcedures",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "EmergencyProcedures",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EmergencyProcedures",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "EmergencyProcedures",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastModifierUserId",
                table: "EmergencyProcedures",
                type: "bigint",
                nullable: true);
        }
    }
}
