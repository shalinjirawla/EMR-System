using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMRSystem.Migrations
{
    /// <inheritdoc />
    public partial class addtablebeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BedId",
                table: "Admissions",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Beds",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BedNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RoomId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Beds_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_BedId",
                table: "Admissions",
                column: "BedId");

            migrationBuilder.CreateIndex(
                name: "IX_Beds_RoomId_BedNumber",
                table: "Beds",
                columns: new[] { "RoomId", "BedNumber" },
                unique: true,
                filter: "[BedNumber] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_Beds_BedId",
                table: "Admissions",
                column: "BedId",
                principalTable: "Beds",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_Beds_BedId",
                table: "Admissions");

            migrationBuilder.DropTable(
                name: "Beds");

            migrationBuilder.DropIndex(
                name: "IX_Admissions_BedId",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "BedId",
                table: "Admissions");
        }
    }
}
