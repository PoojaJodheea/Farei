using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormRequest.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "FormReqDb",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormReqDb_UserId",
                table: "FormReqDb",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormReqDb_AspNetUsers_UserId",
                table: "FormReqDb",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormReqDb_AspNetUsers_UserId",
                table: "FormReqDb");

            migrationBuilder.DropIndex(
                name: "IX_FormReqDb_UserId",
                table: "FormReqDb");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FormReqDb");
        }
    }
}
