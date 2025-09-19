using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class conflictt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MedicineMasters_TenantId_Name_MedicineFormId_Strength_StrengthUnitId",
                table: "MedicineMasters");

            // 2. Alter column
            migrationBuilder.AlterColumn<long>(
                name: "StrengthUnitId",
                table: "MedicineMasters",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            // 3. Recreate index
            migrationBuilder.CreateIndex(
                name: "IX_MedicineMasters_TenantId_Name_MedicineFormId_Strength_StrengthUnitId",
                table: "MedicineMasters",
                columns: new[] { "TenantId", "Name", "MedicineFormId", "Strength", "StrengthUnitId" },
                unique: true,
                filter: "[StrengthUnitId] IS NOT NULL"); // because column is now nullable
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Drop index
            migrationBuilder.DropIndex(
                name: "IX_MedicineMasters_TenantId_Name_MedicineFormId_Strength_StrengthUnitId",
                table: "MedicineMasters");

            // 2. Revert column
            migrationBuilder.AlterColumn<long>(
                name: "StrengthUnitId",
                table: "MedicineMasters",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            // 3. Recreate original index (non-nullable so no filter)
            migrationBuilder.CreateIndex(
                name: "IX_MedicineMasters_TenantId_Name_MedicineFormId_Strength_StrengthUnitId",
                table: "MedicineMasters",
                columns: new[] { "TenantId", "Name", "MedicineFormId", "Strength", "StrengthUnitId" },
                unique: true);
        }
    }
}
