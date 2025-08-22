using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class addemergency2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AdmissionsId",
                table: "EmergencyCases",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyCases_AdmissionsId",
                table: "EmergencyCases",
                column: "AdmissionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyCases_Admissions_AdmissionsId",
                table: "EmergencyCases",
                column: "AdmissionsId",
                principalTable: "Admissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyCases_Admissions_AdmissionsId",
                table: "EmergencyCases");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyCases_AdmissionsId",
                table: "EmergencyCases");

            migrationBuilder.DropColumn(
                name: "AdmissionsId",
                table: "EmergencyCases");
        }
    }
}
