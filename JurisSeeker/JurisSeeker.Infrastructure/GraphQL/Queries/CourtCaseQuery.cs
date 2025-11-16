using HotChocolate.Language;
using JurisSeeker.Core.Interfaces;
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
    /// Получить пользователей
    /// </summary>
    /// <param name="context">Контекст данных</param>
    /// <returns>Пользователи</returns>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [GraphQLDescription("Получить пользователей")]
    public IQueryable<CourtCase> GetCourtCases(IDbContext context) => context.CourtCases.AsNoTracking();


    /// <summary>
    /// Получить пользователей
    /// </summary>
    /// <param name="context">Контекст данных</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Пользователи</returns>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [GraphQLDescription("Получить пользователей")]
    public async Task<List<CourtDecisionDocument>> GetCourtCasesFromElastic(IElasticsearchContext context,
        CancellationToken cancellationToken) => await context.GetAllCourtDecisionsAsync(cancellationToken);

}