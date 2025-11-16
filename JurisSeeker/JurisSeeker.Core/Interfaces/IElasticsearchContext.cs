using JurisSeeker.Domain.ElasticEntity;

namespace JurisSeeker.Core.Interfaces;

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
    /// <param name="id">Id сущности из постгреса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task<CourtDecisionDocument?> GetCourtDecisionAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить все документы из индекса
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Список всех документов CourtDecisionDocument</returns>
    Task<List<CourtDecisionDocument>> GetAllCourtDecisionsAsync(CancellationToken cancellationToken = default);
}