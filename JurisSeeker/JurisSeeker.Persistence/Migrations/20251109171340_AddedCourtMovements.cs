using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JurisSeeker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedCourtMovements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourtCaseMovements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventName = table.Column<string>(type: "text", nullable: false),
                    EventResult = table.Column<string>(type: "text", nullable: false),
                    Basis = table.Column<string>(type: "text", nullable: false),
                    EventDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CourtCaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourtCaseMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourtCaseMovements_CourtCases_CourtCaseId",
                        column: x => x.CourtCaseId,
                        principalTable: "CourtCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourtCaseMovements_CourtCaseId",
                table: "CourtCaseMovements",
                column: "CourtCaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourtCaseMovements");
        }
    }
}
