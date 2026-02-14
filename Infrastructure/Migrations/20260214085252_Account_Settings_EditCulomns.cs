using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Account_Settings_EditCulomns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Single_IsEnable",
                table: "Accounts",
                newName: "PaymentSettings_Single_IsEnable");

            migrationBuilder.RenameColumn(
                name: "Single_ContractExpire",
                table: "Accounts",
                newName: "PaymentSettings_Single_ContractExpire");

            migrationBuilder.RenameColumn(
                name: "Batch_MinSatnaAmount",
                table: "Accounts",
                newName: "PaymentSettings_Batch_MinSatnaAmount");

            migrationBuilder.RenameColumn(
                name: "Batch_IsEnable",
                table: "Accounts",
                newName: "PaymentSettings_Batch_IsEnable");

            migrationBuilder.RenameColumn(
                name: "Batch_ContractExpire",
                table: "Accounts",
                newName: "PaymentSettings_Batch_ContractExpire");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ExpirationDate",
                table: "Accounts",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentSettings_Single_IsEnable",
                table: "Accounts",
                newName: "Single_IsEnable");

            migrationBuilder.RenameColumn(
                name: "PaymentSettings_Single_ContractExpire",
                table: "Accounts",
                newName: "Single_ContractExpire");

            migrationBuilder.RenameColumn(
                name: "PaymentSettings_Batch_MinSatnaAmount",
                table: "Accounts",
                newName: "Batch_MinSatnaAmount");

            migrationBuilder.RenameColumn(
                name: "PaymentSettings_Batch_IsEnable",
                table: "Accounts",
                newName: "Batch_IsEnable");

            migrationBuilder.RenameColumn(
                name: "PaymentSettings_Batch_ContractExpire",
                table: "Accounts",
                newName: "Batch_ContractExpire");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ExpirationDate",
                table: "Accounts",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);
        }
    }
}
