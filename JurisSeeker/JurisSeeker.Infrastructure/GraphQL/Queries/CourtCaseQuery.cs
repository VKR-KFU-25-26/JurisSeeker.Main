using HotChocolate.Language;
using JurisSeeker.Application.Interfaces;
using JurisSeeker.Domain.ElasticEntity;
using JurisSeeker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JurisSeeker.Infrastructure.GraphQL.Queries;

/// <summary>
/// Для работы с судебными делами
/// </summary>
[ExtendObjectType(OperationType.Query)]
public class CourtCaseQuery
{
    /// <summary>
    /// Получить дела
    /// </summary>
    /// <param name="context">Контекст данных</param>
    /// <returns>Пользователи</returns>
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [GraphQLDescription("Получить Дела")]
    public IQueryable<CourtCase> GetCourtCases([Service] IDbContext context) => context.CourtCases.AsNoTracking();
    
    [GraphQLDescription("Получить дела по IDs")]
    [UseFiltering]
    [UseSorting]
    public IQueryable<CourtCase> GetCourtCasesByIds(
        [Service] IDbContext context,
        [ID] Guid[] ids)
    {
        return context.CourtCases
            .Where(x => ids.Contains(x.Id))
            .AsNoTracking();
    }

    /// <summary>
    /// Получить дело по Id из Postgres
    /// </summary>
    /// <param name="context">Контекст данных</param>
    /// <param name="id">Id дела</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Пользователь</returns>
    [GraphQLDescription("Получить дело по Id")]
    public async Task<CourtCase?> GetCourtCaseById([Service] IDbContext context, Guid id, CancellationToken cancellationToken)
        => await context.CourtCases
            .Include(x => x.Articles)
            .Include(x => x.Movements)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    /// <summary>
    /// Получить дело по Id из ElasticSearch
    /// </summary>
    /// <param name="elasticsearchContext">ElasticSearch Context</param>
    /// <param name="courtCaseId">Id дела</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Пользователь</returns>
    [GraphQLDescription("Получить дело по Id из Эластика")]
    public async Task<CourtDecisionDocument?> GetCourtCaseByCourtCaseId(
        string courtCaseId,
        [Service] IElasticsearchContext elasticsearchContext,
        CancellationToken cancellationToken)
        => await elasticsearchContext.GetCourtDecisionByIdAsync(courtCaseId, cancellationToken);
    
    /// <summary>
    /// Получить статьи по делу
    /// </summary>
    /// <param name="context">Контекст данных</param>
    /// <param name="courtCaseId">Id дела</param>
    /// <returns>Cтатьи по делу</returns>
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [GraphQLDescription("Получить статьи дела")]
    public IQueryable<CaseArticle> GetCaseArticles(IDbContext context, Guid courtCaseId)
        => context.CaseArticles.Where(a => a.CourtCaseId == courtCaseId);

    /// <summary>
    /// Получить движение дела 
    /// </summary>
    /// <param name="context">Контекст данных</param>
    /// <param name="courtCaseId">Id дела</param>
    /// <returns>Движение дела</returns>
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [GraphQLDescription("Получить движения дела")]
    public IQueryable<CourtCaseMovement> GetCourtCaseMovements(IDbContext context, Guid courtCaseId)
        => context.CourtCaseMovements.Where(m => m.CourtCaseId == courtCaseId);
    
    /// <summary>
    /// Получить дела из эластика
    /// </summary>
    /// <param name="context">Контекст данных</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Пользователи</returns>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [GraphQLDescription("Получить дела из эластика")]
    public async Task<List<CourtDecisionDocument>> GetCourtCasesFromElastic(IElasticsearchContext context,
        CancellationToken cancellationToken) => await context.GetAllCourtDecisionsAsync(cancellationToken);

}