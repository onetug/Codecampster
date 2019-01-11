using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Codecamp.Migrations
{
    public partial class AddedTimeslotsTracksAndSponsorsToDbContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Timeslot_TimeslotId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Track_TrackId",
                table: "Sessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Track",
                table: "Track");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Timeslot",
                table: "Timeslot");

            migrationBuilder.RenameTable(
                name: "Track",
                newName: "Tracks");

            migrationBuilder.RenameTable(
                name: "Timeslot",
                newName: "Timeslots");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tracks",
                table: "Tracks",
                column: "TrackId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Timeslots",
                table: "Timeslots",
                column: "TimeslotId");

            migrationBuilder.CreateTable(
                name: "Sponsors",
                columns: table => new
                {
                    SponsorId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyName = table.Column<string>(nullable: true),
                    SponsorLevel = table.Column<int>(nullable: false),
                    Bio = table.Column<string>(nullable: true),
                    TwitterHandle = table.Column<string>(nullable: true),
                    WebsiteUrl = table.Column<string>(nullable: true),
                    Image = table.Column<byte[]>(nullable: true),
                    PointOfContact = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sponsors", x => x.SponsorId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Timeslots_TimeslotId",
                table: "Sessions",
                column: "TimeslotId",
                principalTable: "Timeslots",
                principalColumn: "TimeslotId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Tracks_TrackId",
                table: "Sessions",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "TrackId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Timeslots_TimeslotId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Tracks_TrackId",
                table: "Sessions");

            migrationBuilder.DropTable(
                name: "Sponsors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tracks",
                table: "Tracks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Timeslots",
                table: "Timeslots");

            migrationBuilder.RenameTable(
                name: "Tracks",
                newName: "Track");

            migrationBuilder.RenameTable(
                name: "Timeslots",
                newName: "Timeslot");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Track",
                table: "Track",
                column: "TrackId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Timeslot",
                table: "Timeslot",
                column: "TimeslotId");

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
    }
}
