using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class updatepatienttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DischargeDate",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "InsurancePolicyNumber",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "InsuranceProvider",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "IsAdmitted",
                table: "Patients");

            migrationBuilder.AddColumn<int>(
                name: "BillingMethod",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "Patients",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MedicineName",
                table: "MedicineOrderItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingMethod",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "MedicineName",
                table: "MedicineOrderItems");

            migrationBuilder.AddColumn<DateTime>(
                name: "DischargeDate",
                table: "Patients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InsurancePolicyNumber",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InsuranceProvider",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmitted",
                table: "Patients",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
