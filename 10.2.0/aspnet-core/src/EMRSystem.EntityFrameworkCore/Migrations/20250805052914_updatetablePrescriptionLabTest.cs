using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class updatetablePrescriptionLabTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "PrescriptionId",
                table: "PrescriptionLabTests",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "PatientId",
                table: "PrescriptionLabTests",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionLabTests_PatientId",
                table: "PrescriptionLabTests",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionLabTests_Patients_PatientId",
                table: "PrescriptionLabTests",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionLabTests_Patients_PatientId",
                table: "PrescriptionLabTests");

            migrationBuilder.DropIndex(
                name: "IX_PrescriptionLabTests_PatientId",
                table: "PrescriptionLabTests");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "PrescriptionLabTests");

            migrationBuilder.AlterColumn<long>(
                name: "PrescriptionId",
                table: "PrescriptionLabTests",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
