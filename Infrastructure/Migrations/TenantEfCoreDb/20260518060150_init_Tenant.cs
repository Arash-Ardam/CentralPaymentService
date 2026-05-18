using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations.TenantEfCoreDb
{
    /// <inheritdoc />
    public partial class init_Tenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrackingCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProviderMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specifics_NumberOfTransactions = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    Specifics_Status = table.Column<int>(type: "int", nullable: true),
                    SourceAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupedTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackingId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProviderMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Specs_Amount = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    Specs_Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specs_FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specs_LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specs_AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specs_Iban = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specs_NationalId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specs_PaymentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specs_TransactionType = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupedTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupedTransactions_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SingleTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specs_Amount = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    Specs_Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specs_FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specs_LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specs_AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specs_PaymentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specs_TransactionType = table.Column<int>(type: "int", nullable: false, defaultValue: 4)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SingleTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SingleTransactions_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupedTransactions_OrderId",
                table: "GroupedTransactions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SingleTransactions_OrderId",
                table: "SingleTransactions",
                column: "OrderId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupedTransactions");

            migrationBuilder.DropTable(
                name: "SingleTransactions");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
