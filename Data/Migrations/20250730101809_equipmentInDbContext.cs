using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormRequest.Data.Migrations
{
    /// <inheritdoc />
    public partial class equipmentInDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormReqDb_EquipmentInventory_EquipmentID",
                table: "FormReqDb");

            migrationBuilder.DropForeignKey(
                name: "FK_Registry_EquipmentInventory_EquipmentID",
                table: "Registry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EquipmentInventory",
                table: "EquipmentInventory");

            migrationBuilder.RenameTable(
                name: "EquipmentInventory",
                newName: "Equipment");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Equipment",
                table: "Equipment",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_FormReqDb_Equipment_EquipmentID",
                table: "FormReqDb",
                column: "EquipmentID",
                principalTable: "Equipment",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Registry_Equipment_EquipmentID",
                table: "Registry",
                column: "EquipmentID",
                principalTable: "Equipment",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormReqDb_Equipment_EquipmentID",
                table: "FormReqDb");

            migrationBuilder.DropForeignKey(
                name: "FK_Registry_Equipment_EquipmentID",
                table: "Registry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Equipment",
                table: "Equipment");

            migrationBuilder.RenameTable(
                name: "Equipment",
                newName: "EquipmentInventory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquipmentInventory",
                table: "EquipmentInventory",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_FormReqDb_EquipmentInventory_EquipmentID",
                table: "FormReqDb",
                column: "EquipmentID",
                principalTable: "EquipmentInventory",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Registry_EquipmentInventory_EquipmentID",
                table: "Registry",
                column: "EquipmentID",
                principalTable: "EquipmentInventory",
                principalColumn: "ID");
        }
    }
}
