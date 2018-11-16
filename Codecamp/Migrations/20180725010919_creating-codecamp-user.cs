using Microsoft.EntityFrameworkCore.Migrations;

namespace Codecamp.Migrations
{
    public partial class creatingcodecampuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeographicLocation",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAttending",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVolunteer",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TwitterHandle",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GeographicLocation",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsAttending",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsVolunteer",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TwitterHandle",
                table: "AspNetUsers");
        }
    }
}
