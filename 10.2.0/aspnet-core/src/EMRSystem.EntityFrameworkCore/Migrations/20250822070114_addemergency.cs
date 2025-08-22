using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class addemergency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pulse",
                table: "Triages");

            migrationBuilder.RenameColumn(
                name: "AssessmentTime",
                table: "Triages",
                newName: "Time");

            migrationBuilder.AlterColumn<float>(
                name: "Temperature",
                table: "Triages",
                type: "real",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "RespiratoryRate",
                table: "Triages",
                type: "real",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "BloodPressureSystolic",
                table: "Triages",
                type: "real",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "BloodPressureDiastolic",
                table: "Triages",
                type: "real",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<float>(
                name: "HeartRate",
                table: "Triages",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NurseId",
                table: "Triages",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "OxygenSaturation",
                table: "Triages",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Severity",
                table: "Triages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DischargeTime",
                table: "EmergencyCases",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Triages_NurseId",
                table: "Triages",
                column: "NurseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Triages_Nurses_NurseId",
                table: "Triages",
                column: "NurseId",
                principalTable: "Nurses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Triages_Nurses_NurseId",
                table: "Triages");

            migrationBuilder.DropIndex(
                name: "IX_Triages_NurseId",
                table: "Triages");

            migrationBuilder.DropColumn(
                name: "HeartRate",
                table: "Triages");

            migrationBuilder.DropColumn(
                name: "NurseId",
                table: "Triages");

            migrationBuilder.DropColumn(
                name: "OxygenSaturation",
                table: "Triages");

            migrationBuilder.DropColumn(
                name: "Severity",
                table: "Triages");

            migrationBuilder.DropColumn(
                name: "DischargeTime",
                table: "EmergencyCases");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "Triages",
                newName: "AssessmentTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "Temperature",
                table: "Triages",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RespiratoryRate",
                table: "Triages",
                type: "int",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BloodPressureSystolic",
                table: "Triages",
                type: "int",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BloodPressureDiastolic",
                table: "Triages",
                type: "int",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pulse",
                table: "Triages",
                type: "int",
                nullable: true);
        }
    }
}
