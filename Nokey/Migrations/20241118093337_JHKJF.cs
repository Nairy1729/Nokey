using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nokey.Migrations
{
    public partial class JHKJF : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Profile_Bio",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Profile_ProfilePhoto",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Profile_Resume",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Profile_ResumeFileName",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Profile_Skills",
                table: "Persons");

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Skills = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Resume = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ResumeFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfilePhoto = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profiles_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_PersonId",
                table: "Profiles",
                column: "PersonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.AddColumn<string>(
                name: "Profile_Bio",
                table: "Persons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Profile_ProfilePhoto",
                table: "Persons",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Profile_Resume",
                table: "Persons",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Profile_ResumeFileName",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Profile_Skills",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
