using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class addSpecialistDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DepartmentId",
                table: "Prescriptions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSpecialAdviceRequired",
                table: "Prescriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "SpecialistDoctorId",
                table: "Prescriptions",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_DepartmentId",
                table: "Prescriptions",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_SpecialistDoctorId",
                table: "Prescriptions",
                column: "SpecialistDoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescriptions_Departments_DepartmentId",
                table: "Prescriptions",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescriptions_Doctors_SpecialistDoctorId",
                table: "Prescriptions",
                column: "SpecialistDoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prescriptions_Departments_DepartmentId",
                table: "Prescriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescriptions_Doctors_SpecialistDoctorId",
                table: "Prescriptions");

            migrationBuilder.DropIndex(
                name: "IX_Prescriptions_DepartmentId",
                table: "Prescriptions");

            migrationBuilder.DropIndex(
                name: "IX_Prescriptions_SpecialistDoctorId",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "IsSpecialAdviceRequired",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "SpecialistDoctorId",
                table: "Prescriptions");
        }
    }
}
