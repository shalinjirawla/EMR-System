using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdmissionAndPatientDischargeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCharges",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "TotalDeposits",
                table: "Admissions");

            migrationBuilder.AddColumn<string>(
                name: "Activity",
                table: "PatientDischarges",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConditionAtDischarge",
                table: "PatientDischarges",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DietAdvice",
                table: "PatientDischarges",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinalDiagnosis",
                table: "PatientDischarges",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FollowUpDate",
                table: "PatientDischarges",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FollowUpDoctorId",
                table: "PatientDischarges",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvestigationSummary",
                table: "PatientDischarges",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProvisionalDiagnosis",
                table: "PatientDischarges",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReasonForAdmit",
                table: "Admissions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientDischarges_FollowUpDoctorId",
                table: "PatientDischarges",
                column: "FollowUpDoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientDischarges_Doctors_FollowUpDoctorId",
                table: "PatientDischarges",
                column: "FollowUpDoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientDischarges_Doctors_FollowUpDoctorId",
                table: "PatientDischarges");

            migrationBuilder.DropIndex(
                name: "IX_PatientDischarges_FollowUpDoctorId",
                table: "PatientDischarges");

            migrationBuilder.DropColumn(
                name: "Activity",
                table: "PatientDischarges");

            migrationBuilder.DropColumn(
                name: "ConditionAtDischarge",
                table: "PatientDischarges");

            migrationBuilder.DropColumn(
                name: "DietAdvice",
                table: "PatientDischarges");

            migrationBuilder.DropColumn(
                name: "FinalDiagnosis",
                table: "PatientDischarges");

            migrationBuilder.DropColumn(
                name: "FollowUpDate",
                table: "PatientDischarges");

            migrationBuilder.DropColumn(
                name: "FollowUpDoctorId",
                table: "PatientDischarges");

            migrationBuilder.DropColumn(
                name: "InvestigationSummary",
                table: "PatientDischarges");

            migrationBuilder.DropColumn(
                name: "ProvisionalDiagnosis",
                table: "PatientDischarges");

            migrationBuilder.DropColumn(
                name: "ReasonForAdmit",
                table: "Admissions");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCharges",
                table: "Admissions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDeposits",
                table: "Admissions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
