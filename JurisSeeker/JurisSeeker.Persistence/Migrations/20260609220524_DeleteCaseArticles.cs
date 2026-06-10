using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JurisSeeker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DeleteCaseArticles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaseArticles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CaseArticles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourtCaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    ArticleNumber = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Outcome = table.Column<int>(type: "integer", nullable: false),
                    Part = table.Column<string>(type: "text", nullable: true),
                    Point = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseArticles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseArticles_CourtCases_CourtCaseId",
                        column: x => x.CourtCaseId,
                        principalTable: "CourtCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaseArticles_ArticleNumber",
                table: "CaseArticles",
                column: "ArticleNumber");

            migrationBuilder.CreateIndex(
                name: "IX_CaseArticles_ArticleNumber_Outcome",
                table: "CaseArticles",
                columns: new[] { "ArticleNumber", "Outcome" });

            migrationBuilder.CreateIndex(
                name: "IX_CaseArticles_CourtCaseId",
                table: "CaseArticles",
                column: "CourtCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseArticles_Outcome",
                table: "CaseArticles",
                column: "Outcome");
        }
    }
}
