using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RightBrothersProduction.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddingIsPublicAttributeToRequestLogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "RequestLogs",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "RequestLogs");
        }
    }
}
