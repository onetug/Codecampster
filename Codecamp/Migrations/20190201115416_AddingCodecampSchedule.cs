using Microsoft.EntityFrameworkCore.Migrations;

namespace Codecamp.Migrations
{
    public partial class AddingCodecampSchedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CodecampSchedule",
                columns: table => new
                {
                    SessionId = table.Column<int>(nullable: false),
                    TrackId = table.Column<int>(nullable: false),
                    TimeslotId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodecampSchedule", x => new { x.SessionId, x.TrackId, x.TimeslotId });
                    table.ForeignKey(
                        name: "FK_CodecampSchedule_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CodecampSchedule_Timeslots_TimeslotId",
                        column: x => x.TimeslotId,
                        principalTable: "Timeslots",
                        principalColumn: "TimeslotId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CodecampSchedule_Tracks_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Tracks",
                        principalColumn: "TrackId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CodecampSchedule_TimeslotId",
                table: "CodecampSchedule",
                column: "TimeslotId");

            migrationBuilder.CreateIndex(
                name: "IX_CodecampSchedule_TrackId",
                table: "CodecampSchedule",
                column: "TrackId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CodecampSchedule");
        }
    }
}
