using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormRequest.Data.Migrations
{
    /// <inheritdoc />
    public partial class addingStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registry_FormReqDb_FormReqDbId",
                table: "Registry");

            migrationBuilder.AlterColumn<int>(
                name: "FormReqDbId",
                table: "Registry",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "FormReqDb",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Registry_FormReqDb_FormReqDbId",
                table: "Registry",
                column: "FormReqDbId",
                principalTable: "FormReqDb",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registry_FormReqDb_FormReqDbId",
                table: "Registry");

            migrationBuilder.DropColumn(
                name: "status",
                table: "FormReqDb");

            migrationBuilder.AlterColumn<int>(
                name: "FormReqDbId",
                table: "Registry",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Registry_FormReqDb_FormReqDbId",
                table: "Registry",
                column: "FormReqDbId",
                principalTable: "FormReqDb",
                principalColumn: "Id");
        }
    }
}
