using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RightBrothersProduction.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddingTheIsRegisteredColumnToRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRegistered",
                table: "Requests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRegistered",
                table: "Requests");
        }
    }
}
