using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JurisSeeker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DeleteUselessProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DecisionDate",
                table: "CourtCases");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CourtCases");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "CourtCases");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DecisionDate",
                table: "CourtCases",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CourtCases",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "CourtCases",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
