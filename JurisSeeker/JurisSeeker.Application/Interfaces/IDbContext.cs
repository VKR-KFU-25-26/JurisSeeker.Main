using JurisSeeker.Domain.Entities;
using JurisSeeker.Domain.Entities.NotMapped;
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
    
    public DbSet<TotalCasesStats> TotalCasesStats { get; set; } 
    
    public DbSet<OutcomeStats> OutcomeStats { get; set; }
    
    public DbSet<DecisionStats> DecisionStats { get; set; }
    
    public DbSet<CasesByRegionStats> CasesByRegionStats { get; set; } 
    
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
    
    /// <summary>
    /// Выполнить SQL команду (не возвращает результат)
    /// </summary>
    Task<int> ExecuteSqlRawAsync(string sql, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Выполнить SQL команду с параметрами (не возвращает результат)
    /// </summary>
    Task<int> ExecuteSqlRawAsync(string sql, object[] parameters, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Выполнить SQL запрос и вернуть результат
    /// </summary>
    IQueryable<T> SqlQueryRaw<T>(string sql, params object[] parameters) where T : class;
}
