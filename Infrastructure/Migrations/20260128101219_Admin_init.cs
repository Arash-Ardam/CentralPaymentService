using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Admin_init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Iban = table.Column<string>(type: "nvarchar(26)", maxLength: 26, nullable: false),
                    ExpirationDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsEnable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Batch_IsEnable = table.Column<bool>(type: "bit", nullable: true),
                    PaymentSettings_Batch_MaxTransactionsCount = table.Column<int>(type: "int", nullable: true),
                    PaymentSettings_Batch_MaxDailyAmount = table.Column<long>(type: "bigint", nullable: true),
                    Batch_MinSatnaAmount = table.Column<long>(type: "bigint", nullable: true),
                    Batch_ContractExpire = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Single_IsEnable = table.Column<bool>(type: "bit", nullable: true),
                    Single_ContractExpire = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PaymentSettings_Single_TerminalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentSettings_Single_MerchantId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentSettings_Single_Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentSettings_Single_Password = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Banks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    isEnable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ServiceTypes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsEnable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    TenantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Info_FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Info_LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Info_NationalCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Banks");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
