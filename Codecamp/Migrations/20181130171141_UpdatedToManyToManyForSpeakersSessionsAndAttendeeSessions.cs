using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Codecamp.Migrations
{
    public partial class UpdatedToManyToManyForSpeakersSessionsAndAttendeeSessions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendeesSessions_AspNetUsers_CodecampUserId",
                table: "AttendeesSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Speakers_SpeakerId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Speakers_Sessions_SessionId",
                table: "Speakers");

            migrationBuilder.DropIndex(
                name: "IX_Speakers_SessionId",
                table: "Speakers");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_SpeakerId",
                table: "Sessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttendeesSessions",
                table: "AttendeesSessions");

            migrationBuilder.DropIndex(
                name: "IX_AttendeesSessions_CodecampUserId",
                table: "AttendeesSessions");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Speakers");

            migrationBuilder.DropColumn(
                name: "SpeakerId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "AttendeeSessionId",
                table: "AttendeesSessions");

            migrationBuilder.AlterColumn<string>(
                name: "CodecampUserId",
                table: "AttendeesSessions",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttendeesSessions",
                table: "AttendeesSessions",
                columns: new[] { "CodecampUserId", "SessionId" });

            migrationBuilder.CreateTable(
                name: "SpeakerSessions",
                columns: table => new
                {
                    SpeakerId = table.Column<int>(nullable: false),
                    SessionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeakerSessions", x => new { x.SpeakerId, x.SessionId });
                    table.ForeignKey(
                        name: "FK_SpeakerSessions_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpeakerSessions_Speakers_SpeakerId",
                        column: x => x.SpeakerId,
                        principalTable: "Speakers",
                        principalColumn: "SpeakerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpeakerSessions_SessionId",
                table: "SpeakerSessions",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendeesSessions_AspNetUsers_CodecampUserId",
                table: "AttendeesSessions",
                column: "CodecampUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendeesSessions_AspNetUsers_CodecampUserId",
                table: "AttendeesSessions");

            migrationBuilder.DropTable(
                name: "SpeakerSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttendeesSessions",
                table: "AttendeesSessions");

            migrationBuilder.AddColumn<int>(
                name: "SessionId",
                table: "Speakers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SpeakerId",
                table: "Sessions",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CodecampUserId",
                table: "AttendeesSessions",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "AttendeeSessionId",
                table: "AttendeesSessions",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttendeesSessions",
                table: "AttendeesSessions",
                column: "AttendeeSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Speakers_SessionId",
                table: "Speakers",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_SpeakerId",
                table: "Sessions",
                column: "SpeakerId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendeesSessions_CodecampUserId",
                table: "AttendeesSessions",
                column: "CodecampUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendeesSessions_AspNetUsers_CodecampUserId",
                table: "AttendeesSessions",
                column: "CodecampUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Speakers_SpeakerId",
                table: "Sessions",
                column: "SpeakerId",
                principalTable: "Speakers",
                principalColumn: "SpeakerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Speakers_Sessions_SessionId",
                table: "Speakers",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "SessionId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
