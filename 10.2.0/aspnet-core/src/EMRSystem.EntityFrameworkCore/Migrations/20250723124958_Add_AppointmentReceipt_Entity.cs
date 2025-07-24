using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class Add_AppointmentReceipt_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Nurses_NurseId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_NurseId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "NurseId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Appointments");

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmitted",
                table: "Patients",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PatientType",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AppointmentReceipts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    AppointmentId = table.Column<long>(type: "bigint", nullable: false),
                    PatientId = table.Column<long>(type: "bigint", nullable: false),
                    DoctorId = table.Column<long>(type: "bigint", nullable: false),
                    ConsultationFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReceiptNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentReceipts_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppointmentReceipts_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppointmentReceipts_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentReceipts_AppointmentId",
                table: "AppointmentReceipts",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentReceipts_DoctorId",
                table: "AppointmentReceipts",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentReceipts_PatientId",
                table: "AppointmentReceipts",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentReceipts_ReceiptNumber",
                table: "AppointmentReceipts",
                column: "ReceiptNumber",
                unique: true,
                filter: "[ReceiptNumber] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentReceipts");

            migrationBuilder.DropColumn(
                name: "IsAdmitted",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PatientType",
                table: "Appointments");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "Appointments",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<long>(
                name: "NurseId",
                table: "Appointments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "Appointments",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_NurseId",
                table: "Appointments",
                column: "NurseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Nurses_NurseId",
                table: "Appointments",
                column: "NurseId",
                principalTable: "Nurses",
                principalColumn: "Id");
        }
    }
}
