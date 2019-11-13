using Microsoft.EntityFrameworkCore.Migrations;

namespace Codecamp.Migrations
{
    public partial class AddedEventIdToTimeslotsAndTracks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Tracks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Timeslots",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_EventId",
                table: "Tracks",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Timeslots_EventId",
                table: "Timeslots",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Timeslots_Events_EventId",
                table: "Timeslots",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_Events_EventId",
                table: "Tracks",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timeslots_Events_EventId",
                table: "Timeslots");

            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Events_EventId",
                table: "Tracks");

            migrationBuilder.DropIndex(
                name: "IX_Tracks_EventId",
                table: "Tracks");

            migrationBuilder.DropIndex(
                name: "IX_Timeslots_EventId",
                table: "Timeslots");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Timeslots");
        }
    }
}
