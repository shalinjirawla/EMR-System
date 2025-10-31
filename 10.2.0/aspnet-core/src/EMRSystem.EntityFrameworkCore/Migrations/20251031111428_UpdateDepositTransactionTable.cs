using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDepositTransactionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRefund",
                table: "DepositTransactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefundDate",
                table: "DepositTransactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundReceiptNo",
                table: "DepositTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundTransactionId",
                table: "DepositTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RefundedAmount",
                table: "DepositTransactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RemainingAmount",
                table: "DepositTransactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRefund",
                table: "DepositTransactions");

            migrationBuilder.DropColumn(
                name: "RefundDate",
                table: "DepositTransactions");

            migrationBuilder.DropColumn(
                name: "RefundReceiptNo",
                table: "DepositTransactions");

            migrationBuilder.DropColumn(
                name: "RefundTransactionId",
                table: "DepositTransactions");

            migrationBuilder.DropColumn(
                name: "RefundedAmount",
                table: "DepositTransactions");

            migrationBuilder.DropColumn(
                name: "RemainingAmount",
                table: "DepositTransactions");
        }
    }
}
