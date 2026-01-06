using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RightBrothersProduction.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ConvertTheNewStatusColumnInRequestLogToBeOfTypeString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NewStatus",
                table: "RequestLogs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "NewStatus",
                table: "RequestLogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
