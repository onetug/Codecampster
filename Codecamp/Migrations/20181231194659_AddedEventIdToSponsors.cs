using Microsoft.EntityFrameworkCore.Migrations;

namespace Codecamp.Migrations
{
    public partial class AddedEventIdToSponsors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Sponsors",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_EventId",
                table: "Sponsors",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sponsors_Events_EventId",
                table: "Sponsors",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sponsors_Events_EventId",
                table: "Sponsors");

            migrationBuilder.DropIndex(
                name: "IX_Sponsors_EventId",
                table: "Sponsors");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Sponsors");
        }
    }
}
