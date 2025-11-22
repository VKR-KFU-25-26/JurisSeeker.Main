
using JurisSeeker.Domain.ElasticEntity;

namespace JurisSeeker.Application.Interfaces;

/// <summary>
/// Сервис для работы с эластиком
/// </summary>
public interface IElasticsearchContext
{
    /// <summary>
    /// Добавить сущность в эластик
    /// </summary>
    /// <param name="document">Сущность дела</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task IndexCourtDecisionDocumentAsync(CourtDecisionDocument document, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Добавить несколько записей в эластик
    /// </summary>
    /// <param name="documents">Сущности делов</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task BulkIndexCourtDecisionDocumentsAsync(List<CourtDecisionDocument> documents, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить записть из эластика
    /// </summary>
    /// <param name="courtCaseId">Id сущности из постгреса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task<CourtDecisionDocument?> GetCourtDecisionByIdAsync(string courtCaseId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить все документы из индекса
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Список всех документов CourtDecisionDocument</returns>
    Task<List<CourtDecisionDocument>> GetAllCourtDecisionsAsync(CancellationToken cancellationToken = default);
    
    
    Task<List<CourtDecisionDocument>> FullTextSearchAsync(string query, int size = 50, CancellationToken cancellationToken = default);

    Task<List<CourtDecisionDocument>> SearchByRegionAsync(string region, int size = 50, CancellationToken cancellationToken = default);

    Task<List<CourtDecisionDocument>> SearchByRegionAndTextAsync(string region, string query, int size = 50, CancellationToken cancellationToken = default);

    Task<List<CourtDecisionDocument>> SearchByDateRangeAsync(DateTime from, DateTime to, int size = 100, CancellationToken cancellationToken = default);

}