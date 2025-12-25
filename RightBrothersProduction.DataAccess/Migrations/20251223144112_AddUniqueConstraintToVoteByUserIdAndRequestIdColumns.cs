using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RightBrothersProduction.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToVoteByUserIdAndRequestIdColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Votes_UserId",
                table: "Votes");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_UserId_RequestId",
                table: "Votes",
                columns: new[] { "UserId", "RequestId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Votes_UserId_RequestId",
                table: "Votes");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_UserId",
                table: "Votes",
                column: "UserId");
        }
    }
}
