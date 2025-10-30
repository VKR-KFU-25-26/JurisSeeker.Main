using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JurisSeeker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourtCases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: false),
                    CaseNumber = table.Column<string>(type: "text", nullable: false),
                    CourtType = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    OriginalCaseLink = table.Column<string>(type: "text", nullable: false),
                    Subject = table.Column<string>(type: "text", nullable: false),
                    HasDecision = table.Column<bool>(type: "boolean", nullable: false),
                    DecisionLink = table.Column<string>(type: "text", nullable: true),
                    DecisionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DecisionType = table.Column<string>(type: "text", nullable: true),
                    FederalDistrict = table.Column<string>(type: "text", nullable: false),
                    Region = table.Column<string>(type: "text", nullable: false),
                    Plaintiff = table.Column<string>(type: "text", nullable: false),
                    Defendant = table.Column<string>(type: "text", nullable: false),
                    ThirdParties = table.Column<string>(type: "text", nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CaseCategory = table.Column<string>(type: "text", nullable: false),
                    CaseSubcategory = table.Column<string>(type: "text", nullable: false),
                    DecisionContent = table.Column<string>(type: "text", nullable: true),
                    Outcome = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourtCases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CaseArticles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ArticleNumber = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Outcome = table.Column<int>(type: "integer", nullable: false),
                    Part = table.Column<string>(type: "text", nullable: true),
                    Point = table.Column<string>(type: "text", nullable: true),
                    CourtCaseId = table.Column<Guid>(type: "uuid", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_CourtCases_CaseNumber",
                table: "CourtCases",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_CourtCases_Outcome",
                table: "CourtCases",
                column: "Outcome");

            migrationBuilder.CreateIndex(
                name: "IX_CourtCases_Region",
                table: "CourtCases",
                column: "Region");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaseArticles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "CourtCases");
        }
    }
}
