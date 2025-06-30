using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormRequest.Data.Migrations
{
    /// <inheritdoc />
    public partial class addingTech : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Technician",
                table: "FormReqDb",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Technician",
                table: "FormReqDb");
        }
    }
}
