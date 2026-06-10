using HotChocolate.Language;
using JurisSeeker.Application.Interfaces;
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
    
    /// <summary>
    /// Полнотекстовый поиск по решениям
    /// </summary>
    [UsePaging]
    [UseProjection]
    [UseSorting]
    [GraphQLDescription("Поиск по тексту судебного решения")]
    public IQueryable<CourtCase> SearchByDecisionContent(
        string searchText,
        [Service] IDbContext context)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return context.CourtCases
                .AsNoTracking()
                .Where(x => false);
        }

        searchText = searchText.Trim().ToLower();

        return context.CourtCases
            .AsNoTracking()
            .Where(x =>
                x.DecisionContent != null &&
                x.DecisionContent
                    .ToLower()
                    .Contains(searchText)
            );
    }
    
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
            .Include(x => x.Movements)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    

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
}