using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JurisSeeker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CaseResult",
                table: "CourtCases",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Representatives",
                table: "CourtCases",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaseResult",
                table: "CourtCases");

            migrationBuilder.DropColumn(
                name: "Representatives",
                table: "CourtCases");
        }
    }
}
