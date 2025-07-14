using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormRequest.Data.Migrations
{
    /// <inheritdoc />
    public partial class dateRec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MovementDate",
                table: "Registry",
                newName: "DateReceived");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateReceived",
                table: "Registry",
                newName: "MovementDate");
        }
    }
}
