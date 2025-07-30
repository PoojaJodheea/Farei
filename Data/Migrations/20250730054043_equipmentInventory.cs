using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormRequest.Data.Migrations
{
    /// <inheritdoc />
    public partial class equipmentInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EquipmentID",
                table: "FormReqDb",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ITTReportsID",
                table: "FormReqDb",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EquipmentInventory",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EquipmentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Site = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentInventory", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormReqDb_EquipmentID",
                table: "FormReqDb",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_FormReqDb_ITTReportsID",
                table: "FormReqDb",
                column: "ITTReportsID");

            migrationBuilder.AddForeignKey(
                name: "FK_FormReqDb_EquipmentInventory_EquipmentID",
                table: "FormReqDb",
                column: "EquipmentID",
                principalTable: "EquipmentInventory",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_FormReqDb_ITTreport_ITTReportsID",
                table: "FormReqDb",
                column: "ITTReportsID",
                principalTable: "ITTreport",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormReqDb_EquipmentInventory_EquipmentID",
                table: "FormReqDb");

            migrationBuilder.DropForeignKey(
                name: "FK_FormReqDb_ITTreport_ITTReportsID",
                table: "FormReqDb");

            migrationBuilder.DropTable(
                name: "EquipmentInventory");

            migrationBuilder.DropIndex(
                name: "IX_FormReqDb_EquipmentID",
                table: "FormReqDb");

            migrationBuilder.DropIndex(
                name: "IX_FormReqDb_ITTReportsID",
                table: "FormReqDb");

            migrationBuilder.DropColumn(
                name: "EquipmentID",
                table: "FormReqDb");

            migrationBuilder.DropColumn(
                name: "ITTReportsID",
                table: "FormReqDb");
        }
    }
}
