using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class addnurseidinvisit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "NurseId",
                table: "Visits",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Visits_NurseId",
                table: "Visits",
                column: "NurseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Nurses_NurseId",
                table: "Visits",
                column: "NurseId",
                principalTable: "Nurses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Nurses_NurseId",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_Visits_NurseId",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "NurseId",
                table: "Visits");
        }
    }
}
