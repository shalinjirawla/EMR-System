using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class addtableprocedurerecipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "SelectedEmergencyProcedures",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<long>(
                name: "ProcedureReceiptId",
                table: "SelectedEmergencyProcedures",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProcedureReceipts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<long>(type: "bigint", nullable: false),
                    TotalFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReceiptNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PaymentIntentId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcedureReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcedureReceipts_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SelectedEmergencyProcedures_ProcedureReceiptId",
                table: "SelectedEmergencyProcedures",
                column: "ProcedureReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureReceipts_PatientId",
                table: "ProcedureReceipts",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureReceipts_ReceiptNumber",
                table: "ProcedureReceipts",
                column: "ReceiptNumber",
                unique: true,
                filter: "[ReceiptNumber] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_SelectedEmergencyProcedures_ProcedureReceipts_ProcedureReceiptId",
                table: "SelectedEmergencyProcedures",
                column: "ProcedureReceiptId",
                principalTable: "ProcedureReceipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SelectedEmergencyProcedures_ProcedureReceipts_ProcedureReceiptId",
                table: "SelectedEmergencyProcedures");

            migrationBuilder.DropTable(
                name: "ProcedureReceipts");

            migrationBuilder.DropIndex(
                name: "IX_SelectedEmergencyProcedures_ProcedureReceiptId",
                table: "SelectedEmergencyProcedures");

            migrationBuilder.DropColumn(
                name: "ProcedureReceiptId",
                table: "SelectedEmergencyProcedures");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "SelectedEmergencyProcedures",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }
    }
}
