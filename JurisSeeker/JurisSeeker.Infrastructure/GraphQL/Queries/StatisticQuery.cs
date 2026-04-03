using HotChocolate.Language;
using JurisSeeker.Application.Dto.Analytics;
using JurisSeeker.Application.Interfaces;
using JurisSeeker.Domain.Entities.NotMapped;
using Microsoft.EntityFrameworkCore;

namespace JurisSeeker.Infrastructure.GraphQL.Queries;

[ExtendObjectType(OperationType.Query)]
public class StatisticQuery
{
    /// <summary>
    /// Получить общую статистику по всем делам
    /// </summary>
    [GraphQLDescription("Получить общую статистику по всем делам")]
    [UseFirstOrDefault]
    public IQueryable<TotalCasesStats> GetTotalCasesStats([Service] IDbContext context) 
        => context.TotalCasesStats.AsNoTracking();
    
    /// <summary>
    /// Получить статистику по регионам - сколько дел в каждом
    /// </summary>
    [GraphQLDescription("Получить статистику по регионам - сколько дел в каждом")]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<CasesByRegionStats> GetCasesByRegionStats([Service] IDbContext context) 
        => context.CasesByRegionStats
            .AsNoTracking()
            .OrderByDescending(x => x.CasesInRegion);
    
    /// <summary>
    /// Получить статистику по решениям - процент дел с решениями
    /// </summary>
    [GraphQLDescription("Получить статистику по решениям - процент дел с решениями")]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<DecisionStats> GetDecisionStats([Service] IDbContext context) 
        => context.DecisionStats
            .AsNoTracking()
            .OrderByDescending(x => x.DecisionRate);
    
    /// <summary>
    /// Получить статистику по исходам дел - кто выигрывает
    /// </summary>
    [GraphQLDescription("Получить статистику по исходам дел - кто выигрывает")]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<OutcomeStats> GetOutcomeStats([Service] IDbContext context) 
        => context.OutcomeStats
            .AsNoTracking()
            .OrderByDescending(x => x.CasesWithKnownOutcome);
    
    /// <summary>
    /// Получить статистику по федеральному округу
    /// </summary>
    [GraphQLDescription("Получить статистику по федеральному округу")]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<OutcomeStats> GetOutcomeStatsByDistrict(
        [Service] IDbContext context,
        string? federalDistrict = null) 
    {
        var query = context.OutcomeStats.AsNoTracking();
        
        if (!string.IsNullOrEmpty(federalDistrict))
        {
            query = query.Where(x => x.FederalDistrict == federalDistrict);
        }
        
        return query.OrderBy(x => x.Region);
    }
    
    /// <summary>
    /// Получить топ регионов по количеству дел
    /// </summary>
    [GraphQLDescription("Получить топ регионов по количеству дел")]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<CasesByRegionStats> GetTopRegionsByCases(
        [Service] IDbContext context,
        int limit = 10) 
        => context.CasesByRegionStats
            .AsNoTracking()
            .OrderByDescending(x => x.CasesInRegion)
            .Take(limit);
    
    /// <summary>
    /// Получить регионы с самым высоким процентом решений
    /// </summary>
    [GraphQLDescription("Получить регионы с самым высоким процентом решений")]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<DecisionStats> GetTopRegionsByDecisionRate(
        [Service] IDbContext context,
        int minCases = 10,
        int limit = 10) 
        => context.DecisionStats
            .AsNoTracking()
            .Where(x => x.TotalCases >= minCases)
            .OrderByDescending(x => x.DecisionRate)
            .Take(limit);
    
    /// <summary>
    /// Получить регионы с самым высоким процентом побед истцов
    /// </summary>
    [GraphQLDescription("Получить регионы с самым высоким процентом побед истцов")]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<OutcomeStats> GetTopRegionsByPlaintiffWinRate(
        [Service] IDbContext context,
        int minCases = 10,
        int limit = 10) 
        => context.OutcomeStats
            .AsNoTracking()
            .Where(x => x.CasesWithKnownOutcome >= minCases)
            .OrderByDescending(x => x.PlaintiffWinPercentage)
            .Take(limit);
    
    /// <summary>
    /// Получить регионы с самым высоким процентом побед ответчиков
    /// </summary>
    [GraphQLDescription("Получить регионы с самым высоким процентом побед ответчиков")]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<OutcomeStats> GetTopRegionsByDefendantWinRate(
        [Service] IDbContext context,
        int minCases = 10,
        int limit = 10) 
        => context.OutcomeStats
            .AsNoTracking()
            .Where(x => x.CasesWithKnownOutcome >= minCases)
            .OrderByDescending(x => x.DefendantWinPercentage)
            .Take(limit);
    
    /// <summary>
    /// Получить сводную аналитику для дашборда
    /// </summary>
    [GraphQLDescription("Получить сводную аналитику для дашборда")]
    public async Task<DashboardAnalyticsResponse> GetAnalyticsDashboard(
        [Service] IDbContext context,
        CancellationToken cancellationToken)
    {
        // Выполняем запросы последовательно, а не параллельно
        var totalStats = await context.TotalCasesStats
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    
        var regionStats = await context.CasesByRegionStats
            .AsNoTracking()
            .OrderByDescending(x => x.CasesInRegion)
            .ToListAsync(cancellationToken);
    
        var decisionStats = await context.DecisionStats
            .AsNoTracking()
            .OrderByDescending(x => x.DecisionRate)
            .ToListAsync(cancellationToken);
    
        var outcomeStats = await context.OutcomeStats
            .AsNoTracking()
            .OrderByDescending(x => x.CasesWithKnownOutcome)
            .ToListAsync(cancellationToken);

        return new DashboardAnalyticsResponse
        {
            TotalStats = totalStats,
            RegionStats = regionStats,
            DecisionStats = decisionStats,
            OutcomeStats = outcomeStats,
            CalculatedAt = DateTime.UtcNow
        };
    }
    
}
