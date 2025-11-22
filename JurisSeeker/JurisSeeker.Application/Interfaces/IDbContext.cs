using JurisSeeker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace JurisSeeker.Application.Interfaces;

/// <summary>
/// Интерфейс контекста базы
/// </summary>
public interface IDbContext
{
    /// <summary>
    /// Судебные дела
    /// </summary>
    public DbSet<CourtCase> CourtCases { get; set; }
    
    /// <summary>
    /// Пользователь
    /// </summary>
    public DbSet<User> Users { get; set; }
    
    /// <summary>
    /// Статьи
    /// </summary>
    public DbSet<CaseArticle> CaseArticles { get; set; }
    
    /// <summary>
    /// Движение дела
    /// </summary>
    public DbSet<CourtCaseMovement> CourtCaseMovements { get; set; }
    
    /// <summary>
    /// Начать транзакцию
    /// </summary>
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить текущую транзакцию
    /// </summary>
    IDbContextTransaction? CurrentTransaction { get; }
    
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        
    DbSet<T> Set<T>() where T : class;
}