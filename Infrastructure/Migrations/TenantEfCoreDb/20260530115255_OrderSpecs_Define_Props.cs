using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations.TenantEfCoreDb
{
    /// <inheritdoc />
    public partial class OrderSpecs_Define_Props : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Specifics_Amount",
                table: "Orders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Specifics_Description",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Specifics_Type",
                table: "Orders",
                type: "int",
                nullable: true,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Specifics_Amount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Specifics_Description",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Specifics_Type",
                table: "Orders");
        }
    }
}
