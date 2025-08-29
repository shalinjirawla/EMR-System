using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableLabTechnicianAndDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "LabTechnician");

            migrationBuilder.AddColumn<long>(
                name: "DepartmentId",
                table: "LabTechnician",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentType",
                table: "Departments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_LabTechnician_DepartmentId",
                table: "LabTechnician",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabTechnician_Departments_DepartmentId",
                table: "LabTechnician",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabTechnician_Departments_DepartmentId",
                table: "LabTechnician");

            migrationBuilder.DropIndex(
                name: "IX_LabTechnician_DepartmentId",
                table: "LabTechnician");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "LabTechnician");

            migrationBuilder.DropColumn(
                name: "DepartmentType",
                table: "Departments");

            migrationBuilder.AddColumn<int>(
                name: "Department",
                table: "LabTechnician",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
