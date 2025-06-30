using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormRequest.Data.Migrations
{
    /// <inheritdoc />
    public partial class tpTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThirdParties",
                columns: table => new
                {
                    ThirdPartyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormReqDbId = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyContact = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateSent = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThirdPartyRemarks = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThirdParties", x => x.ThirdPartyId);
                    table.ForeignKey(
                        name: "FK_ThirdParties_FormReqDb_FormReqDbId",
                        column: x => x.FormReqDbId,
                        principalTable: "FormReqDb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThirdParties_FormReqDbId",
                table: "ThirdParties",
                column: "FormReqDbId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThirdParties");
        }
    }
}
