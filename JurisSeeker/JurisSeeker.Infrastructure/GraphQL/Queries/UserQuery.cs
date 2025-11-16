using HotChocolate.Language;
using JurisSeeker.Core.Interfaces;
using JurisSeeker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JurisSeeker.Infrastructure.GraphQL.Queries;

/// <summary>
/// Для работы с пользователями
/// </summary>
[ExtendObjectType(OperationType.Query)]
public class UserQuery
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
    public IQueryable<User> GetUsers(IDbContext context) => context.Users.AsNoTracking();
}