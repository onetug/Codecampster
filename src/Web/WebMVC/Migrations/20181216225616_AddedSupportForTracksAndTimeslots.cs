using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Codecamp.Migrations
{
    public partial class AddedSupportForTracksAndTimeslots : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimeslotId",
                table: "Sessions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrackId",
                table: "Sessions",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Timeslot",
                columns: table => new
                {
                    TimeslotId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    ContainsNoSessions = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timeslot", x => x.TimeslotId);
                });

            migrationBuilder.CreateTable(
                name: "Track",
                columns: table => new
                {
                    TrackId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    RoomNumber = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Track", x => x.TrackId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_TimeslotId",
                table: "Sessions",
                column: "TimeslotId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_TrackId",
                table: "Sessions",
                column: "TrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Timeslot_TimeslotId",
                table: "Sessions",
                column: "TimeslotId",
                principalTable: "Timeslot",
                principalColumn: "TimeslotId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Track_TrackId",
                table: "Sessions",
                column: "TrackId",
                principalTable: "Track",
                principalColumn: "TrackId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Timeslot_TimeslotId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Track_TrackId",
                table: "Sessions");

            migrationBuilder.DropTable(
                name: "Timeslot");

            migrationBuilder.DropTable(
                name: "Track");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_TimeslotId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_TrackId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "TimeslotId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "TrackId",
                table: "Sessions");
        }
    }
}
