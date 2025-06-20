using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class ChangePatientcontextalso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Nurses_NursesId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_NursesId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "NursesId",
                table: "Patients");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_AssignedNurseId",
                table: "Patients",
                column: "AssignedNurseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Nurses_AssignedNurseId",
                table: "Patients",
                column: "AssignedNurseId",
                principalTable: "Nurses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Nurses_AssignedNurseId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_AssignedNurseId",
                table: "Patients");

            migrationBuilder.AddColumn<long>(
                name: "NursesId",
                table: "Patients",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_NursesId",
                table: "Patients",
                column: "NursesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Nurses_NursesId",
                table: "Patients",
                column: "NursesId",
                principalTable: "Nurses",
                principalColumn: "Id");
        }
    }
}
