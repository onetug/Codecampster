using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Codecamp.Migrations
{
    public partial class MovedFromImageUrlToImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Speakers");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Speakers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Speakers");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Speakers",
                nullable: true);
        }
    }
}
