using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Codecamp.Migrations
{
    public partial class additionofspeaker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Speakers",
                columns: table => new
                {
                    SpeakerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyName = table.Column<string>(nullable: true),
                    Bio = table.Column<string>(nullable: true),
                    WebsiteUrl = table.Column<string>(nullable: true),
                    BlogUrl = table.Column<string>(nullable: true),
                    Image = table.Column<byte[]>(type: "VARBINARY(MAX)", nullable: true),
                    NoteToOrganizers = table.Column<string>(nullable: true),
                    IsMvp = table.Column<bool>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: true),
                    LinkedIn = table.Column<string>(nullable: true),
                    CodecampUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Speakers", x => x.SpeakerId);
                    table.ForeignKey(
                        name: "FK_Speakers_AspNetUsers_CodecampUserId",
                        column: x => x.CodecampUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Speakers_CodecampUserId",
                table: "Speakers",
                column: "CodecampUserId",
                unique: true,
                filter: "[CodecampUserId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Speakers");
        }
    }
}
