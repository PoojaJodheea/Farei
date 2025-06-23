using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormRequest.Data.Migrations
{
    /// <inheritdoc />
    public partial class registry3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "From",
                table: "FormReqDb");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "FormReqDb");

            migrationBuilder.DropColumn(
                name: "IsInvalid",
                table: "FormReqDb");

            migrationBuilder.DropColumn(
                name: "MovementDate",
                table: "FormReqDb");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "FormReqDb");

            migrationBuilder.DropColumn(
                name: "To",
                table: "FormReqDb");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "FormReqDb",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Registry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    From = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    To = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MovementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsInvalid = table.Column<bool>(type: "bit", nullable: false),
                    FormReqDbId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Registry_FormReqDb_FormReqDbId",
                        column: x => x.FormReqDbId,
                        principalTable: "FormReqDb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Registry_FormReqDbId",
                table: "Registry",
                column: "FormReqDbId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Registry");

            migrationBuilder.DropColumn(
                name: "status",
                table: "FormReqDb");

            migrationBuilder.AddColumn<string>(
                name: "From",
                table: "FormReqDb",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "FormReqDb",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsInvalid",
                table: "FormReqDb",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "MovementDate",
                table: "FormReqDb",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "FormReqDb",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "To",
                table: "FormReqDb",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
