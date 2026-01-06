using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RightBrothersProduction.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddVoteDateIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Votes_RequestId",
                table: "Votes");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_RequestId_VotedAt",
                table: "Votes",
                columns: new[] { "RequestId", "VotedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Votes_RequestId_VotedAt",
                table: "Votes");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_RequestId",
                table: "Votes",
                column: "RequestId");
        }
    }
}
