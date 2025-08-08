using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormRequest.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedEquipTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Equipment",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "AntiVirusExpiryDatee",
                table: "Equipment",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AntiVirusLicense",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AntiVirusName",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Brcode",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CpuModel",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfPurchase",
                table: "Equipment",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "EquipmentDrive",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EquipmentMake",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EquipmentModel",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MemoryCapacity",
                table: "Equipment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OS_Key",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OfficeKey",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OfficeLogin",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OfficeName",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OfficePassword",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OneDriveEmail",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OneDrivePassword",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OperatingSys",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StorageCapacity",
                table: "Equipment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Supplier",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "User",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "AntiVirusExpiryDatee",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "AntiVirusLicense",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "AntiVirusName",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "Brcode",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "CpuModel",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "DateOfPurchase",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "EquipmentDrive",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "EquipmentMake",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "EquipmentModel",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "MemoryCapacity",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "OS_Key",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "OfficeKey",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "OfficeLogin",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "OfficeName",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "OfficePassword",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "OneDriveEmail",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "OneDrivePassword",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "OperatingSys",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "StorageCapacity",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "Supplier",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "User",
                table: "Equipment");
        }
    }
}
