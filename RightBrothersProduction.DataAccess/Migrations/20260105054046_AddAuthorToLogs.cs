using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RightBrothersProduction.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorToLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "RequestLogs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsBanned",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(150)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestLogs_CreatedById",
                table: "RequestLogs",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestLogs_AspNetUsers_CreatedById",
                table: "RequestLogs",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"
        UPDATE L
        SET L.CreatedById = R.AssignedToId
        FROM RequestLogs L
        INNER JOIN RegisteredRequests R ON L.RequestId = R.RequestId
        WHERE L.CreatedById IS NULL AND R.AssignedToId IS NOT NULL
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestLogs_AspNetUsers_CreatedById",
                table: "RequestLogs");

            migrationBuilder.DropIndex(
                name: "IX_RequestLogs_CreatedById",
                table: "RequestLogs");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "RequestLogs");

            migrationBuilder.AlterColumn<bool>(
                name: "IsBanned",
                table: "AspNetUsers",
                type: "bit",
                nullable: true,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(150)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");
        }
    }
}
