using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JurisSeeker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddednewProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JudgeName",
                table: "CourtCases",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "CourtCases",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JudgeName",
                table: "CourtCases");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "CourtCases");
        }
    }
}
