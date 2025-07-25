using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class Add_IpdChargeEntry_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_Patients_PatientId",
                table: "Admissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Patients_PatientId",
                table: "Deposits");

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentBalance",
                table: "Patients",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastBillingDate",
                table: "Patients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AdmissionId",
                table: "Deposits",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DischargeDateTime",
                table: "Admissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDischarged",
                table: "Admissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

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

            migrationBuilder.CreateTable(
                name: "IpdChargeEntries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    AdmissionId = table.Column<long>(type: "bigint", nullable: false),
                    PatientId = table.Column<long>(type: "bigint", nullable: false),
                    ChargeType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    ReferenceId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IpdChargeEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IpdChargeEntries_Admissions_AdmissionId",
                        column: x => x.AdmissionId,
                        principalTable: "Admissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IpdChargeEntries_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Deposits_AdmissionId",
                table: "Deposits",
                column: "AdmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_IpdChargeEntries_AdmissionId",
                table: "IpdChargeEntries",
                column: "AdmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_IpdChargeEntries_EntryDate",
                table: "IpdChargeEntries",
                column: "EntryDate");

            migrationBuilder.CreateIndex(
                name: "IX_IpdChargeEntries_PatientId",
                table: "IpdChargeEntries",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_IpdChargeEntries_ReferenceId",
                table: "IpdChargeEntries",
                column: "ReferenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_Patients_PatientId",
                table: "Admissions",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Admissions_AdmissionId",
                table: "Deposits",
                column: "AdmissionId",
                principalTable: "Admissions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Patients_PatientId",
                table: "Deposits",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_Patients_PatientId",
                table: "Admissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Admissions_AdmissionId",
                table: "Deposits");

            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Patients_PatientId",
                table: "Deposits");

            migrationBuilder.DropTable(
                name: "IpdChargeEntries");

            migrationBuilder.DropIndex(
                name: "IX_Deposits_AdmissionId",
                table: "Deposits");

            migrationBuilder.DropColumn(
                name: "CurrentBalance",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "LastBillingDate",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "AdmissionId",
                table: "Deposits");

            migrationBuilder.DropColumn(
                name: "DischargeDateTime",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "IsDischarged",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "TotalCharges",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "TotalDeposits",
                table: "Admissions");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_Patients_PatientId",
                table: "Admissions",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Patients_PatientId",
                table: "Deposits",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");
        }
    }
}
