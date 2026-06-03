using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JurisSeeker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalyticsView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            #region TotalCasesStats

            migrationBuilder.Sql(
                """DROP MATERIALIZED VIEW IF EXISTS "TotalCasesStats";""",
                suppressTransaction: true);

            migrationBuilder.Sql(
                """
                CREATE MATERIALIZED VIEW "TotalCasesStats" AS
                SELECT 
                    COUNT(*) AS "TotalCases",
                    COUNT(CASE WHEN "HasDecision" = true THEN 1 END) AS "TotalCasesWithDecision",
                    COALESCE(
                        ROUND(
                            COUNT(CASE WHEN "HasDecision" = true THEN 1 END)::numeric / 
                            NULLIF(COUNT(*), 0) * 100, 2
                        ), 0
                    ) AS "DecisionPercentage",
                    NOW() AS "CalculatedAt"
                FROM "CourtCases";
                """, 
                suppressTransaction: true);

            migrationBuilder.Sql(
                """
                CREATE UNIQUE INDEX "IX_TotalCasesStats_Const" 
                ON "TotalCasesStats" ((1));
                """,
                suppressTransaction: true);
            
            #endregion

            #region CasesByRegionStats

            migrationBuilder.Sql(
                """DROP MATERIALIZED VIEW IF EXISTS "CasesByRegionStats";""",
                suppressTransaction: true);

            migrationBuilder.Sql(
                """
                CREATE MATERIALIZED VIEW "CasesByRegionStats" AS
                SELECT 
                    "Region",
                    "FederalDistrict",
                    COUNT(*) AS "CasesInRegion",
                    COALESCE(
                        ROUND(
                            COUNT(*)::numeric / 
                            NULLIF((SELECT COUNT(*) FROM "CourtCases"), 0) * 100, 2
                        ), 0
                    ) AS "PercentageOfTotal",
                    NOW() AS "CalculatedAt"
                FROM "CourtCases"
                GROUP BY "Region", "FederalDistrict";
                """,
                suppressTransaction: true);
            
            migrationBuilder.Sql(
                """
                CREATE UNIQUE INDEX IF NOT EXISTS "IX_CasesByRegionStats_Region_FederalDistrict" 
                ON "CasesByRegionStats" ("Region", "FederalDistrict");
                """,
                suppressTransaction: true);

            #endregion

            #region DecisionStats

            migrationBuilder.Sql(
                """DROP MATERIALIZED VIEW IF EXISTS "DecisionStats";""",
                suppressTransaction: true);

            migrationBuilder.Sql(
                """
                CREATE MATERIALIZED VIEW "DecisionStats" AS
                SELECT 
                    "FederalDistrict",
                    "Region",
                    COUNT(*) AS "TotalCases",
                    COUNT(CASE WHEN "HasDecision" = true THEN 1 END) AS "CasesWithDecision",
                    COALESCE(
                        ROUND(
                            COUNT(CASE WHEN "HasDecision" = true THEN 1 END)::numeric / 
                            NULLIF(COUNT(*), 0) * 100, 2
                        ), 0
                    ) AS "DecisionRate",
                    NOW() AS "CalculatedAt"
                FROM "CourtCases"
                GROUP BY "FederalDistrict", "Region";
                """,
                suppressTransaction: true);
            
            migrationBuilder.Sql(
                """
                CREATE UNIQUE INDEX IF NOT EXISTS "IX_DecisionStats_Region_FederalDistrict" 
                ON "DecisionStats" ("Region", "FederalDistrict");
                """,
                suppressTransaction: true);

            #endregion

            #region OutcomeStats

            migrationBuilder.Sql(
                """DROP MATERIALIZED VIEW IF EXISTS "OutcomeStats";""",
                suppressTransaction: true);

            migrationBuilder.Sql(
                """
                CREATE MATERIALIZED VIEW "OutcomeStats" AS
                SELECT 
                    "FederalDistrict",
                    "Region",
                    COUNT(CASE WHEN "Outcome" != 0 THEN 1 END) as "CasesWithKnownOutcome",
                    COUNT(CASE WHEN "Outcome" = 1 THEN 1 END) as "PlaintiffWins",
                    COALESCE(
                        ROUND(
                            COUNT(CASE WHEN "Outcome" = 1 THEN 1 END)::numeric / 
                            NULLIF(COUNT(CASE WHEN "Outcome" != 0 THEN 1 END)::numeric, 0) * 100, 2
                        ), 0
                    ) as "PlaintiffWinPercentage",
                    COUNT(CASE WHEN "Outcome" = 2 THEN 1 END) as "DefendantWins",
                    COALESCE(
                        ROUND(
                            COUNT(CASE WHEN "Outcome" = 2 THEN 1 END)::numeric / 
                            NULLIF(COUNT(CASE WHEN "Outcome" != 0 THEN 1 END)::numeric, 0) * 100, 2
                        ), 0
                    ) as "DefendantWinPercentage",
                    COUNT(CASE WHEN "Outcome" = 3 THEN 1 END) as "Settlements",
                    COALESCE(
                        ROUND(
                            COUNT(CASE WHEN "Outcome" = 3 THEN 1 END)::numeric / 
                            NULLIF(COUNT(CASE WHEN "Outcome" != 0 THEN 1 END)::numeric, 0) * 100, 2
                        ), 0
                    ) as "SettlementPercentage",
                    COUNT(CASE WHEN "Outcome" = 4 THEN 1 END) as "Dismissed",
                    COALESCE(
                        ROUND(
                            COUNT(CASE WHEN "Outcome" = 4 THEN 1 END)::numeric / 
                            NULLIF(COUNT(CASE WHEN "Outcome" != 0 THEN 1 END)::numeric, 0) * 100, 2
                        ), 0
                    ) as "DismissedPercentage",
                    NOW() as "CalculatedAt"
                FROM "CourtCases"
                GROUP BY "FederalDistrict", "Region";
                """,
                suppressTransaction: true);
            
            migrationBuilder.Sql(
                """
                CREATE UNIQUE INDEX IF NOT EXISTS "IX_OutcomeStats_Region_FederalDistrict" 
                ON "OutcomeStats" ("Region", "FederalDistrict");
                """,
                suppressTransaction: true);

            #endregion
        }
        
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """DROP MATERIALIZED VIEW IF EXISTS "TotalCasesStats";""", 
                suppressTransaction: true);
            
            migrationBuilder.Sql(
                """DROP MATERIALIZED VIEW IF EXISTS "CasesByRegionStats";""", 
                suppressTransaction: true);
            
            migrationBuilder.Sql(
                """DROP MATERIALIZED VIEW IF EXISTS "DecisionStats";""", 
                suppressTransaction: true);
            
            migrationBuilder.Sql(
                """DROP MATERIALIZED VIEW IF EXISTS "OutcomeStats";""", 
                suppressTransaction: true);
        }
    }
}