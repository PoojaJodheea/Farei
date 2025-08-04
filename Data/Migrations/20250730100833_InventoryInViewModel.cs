using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormRequest.Data.Migrations
{
    /// <inheritdoc />
    public partial class InventoryInViewModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EquipmentID",
                table: "Registry",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Registry_EquipmentID",
                table: "Registry",
                column: "EquipmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Registry_EquipmentInventory_EquipmentID",
                table: "Registry",
                column: "EquipmentID",
                principalTable: "EquipmentInventory",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registry_EquipmentInventory_EquipmentID",
                table: "Registry");

            migrationBuilder.DropIndex(
                name: "IX_Registry_EquipmentID",
                table: "Registry");

            migrationBuilder.DropColumn(
                name: "EquipmentID",
                table: "Registry");
        }
    }
}
